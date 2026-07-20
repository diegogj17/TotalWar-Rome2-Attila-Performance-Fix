// A simple program to set CPU affinity for Attila_original.exe
/* (or any other process for that matter,
 * if you change the GAME_PROCESS_NAME constant to
 * your desired application and build the console app again)
 */
// Put the .exe of this program (or its shortcut) into win+R > shell:startup to make it run at Windows startup
//
// Based on https://github.com/serkan-erol/Set-CPU-Affinity-for-Rome-2 by serkan-erol

// Without setting CPU affinity, some Total War titles get stuck at battle loading screens on many modern systems
// This program makes it easier to set CPU affinity by handling it automatically, when the game is launched

using System.Diagnostics;
using System.Runtime.Versioning;

[assembly: SupportedOSPlatform("windows")]

// Name of the process (without .exe)
const string GAME_PROCESS_NAME = "Attila_original";

/* For the game to run smoothly, we want to limit the number of CPU cores/threads it can use.
 * This does NOT affect overall system settings; it affects only the specific process we target, and only while it is running.
 * CPUs with less than 2 cores are not supported and will exit with an error message.
 * CPUs with 2-5 cores get 1 core disabled,
 * CPUs with 6-8 cores get 2 cores disabled
 * CPUS with more than 8 cores get limited to 8 cores
 */

// Minimum required number of cores
const int MINIMUM_REQUIRED_CORES = 2;

// Moderate amount of cores threshold
const int MODERATE_AMOUNT_OF_CORES = 5;

// Affinity mask for maximum allowed cores/threads to assign
// Suggested: 255 = 1111 1111 in binary (first 8 cores/threads enabled).
const int MAX_ALLOWED_MASK = 255;

// Delay between checks (in milliseconds)
const int CHECK_DELAY_MS = 1000; // 1 second

// However, one may not have as many as 8 cores/threads! So, we calculate based on the total number of cores
static int CalculateAffinityMask()
{
    // Get the number of logical processors (cores/threads)
    int processorCount = Environment.ProcessorCount;
    Console.WriteLine($"Info: Your system has {processorCount} logical processors.");

    // Left shift 1 by the number of processors to create the initial bitmask
    /* This results in 2 to the power of processorCount
     * For example, if processorCount is 4, then:
     * 1 << 4 = 16 (which is 1_0000 in binary)
     */
    int mask = 1 << processorCount;

    /* Adjust settings according to number of processors in the user's PC.
     * Also, if they have less than 2 (only 1 core!); why are they really here?
     * We will disable the use of at least 1 core for this game!
     */

    // Check if the system meets minimum requirements
    if (processorCount < MINIMUM_REQUIRED_CORES)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Warning: Your system has only {processorCount} logical processors.");
        Console.WriteLine($"This application requires at least {MINIMUM_REQUIRED_CORES} cores/threads to function properly.");
        Console.WriteLine("\nPress any key to exit...");
        Console.ResetColor();
        Console.ReadKey();
        Environment.Exit(1); // Exit with error code 1
    }
    // 1 core/thread is left unassigned for CPUs with 3 to 5 cores
    else if (processorCount is >= MINIMUM_REQUIRED_CORES and <= MODERATE_AMOUNT_OF_CORES)
    {
        /* 2 cores -> 1 << 2 = 4  (100) - 2 = 2 (010) -> Only enables core 1
         * 3 cores -> 1 << 3 = 8  (1000) - 2 = 6 (0110) -> Enables cores 1,2
         * 4 cores -> 1 << 4 = 16 (1_0000) - 2 = 14 (0_1110) -> Enables cores 1,2,3
         * 5 cores -> 1 << 5 = 32 (10_0000) - 2 = 30 (01_1110) -> Enables cores 1,2,3,4
         * All of them exclude core 0, leaving it as unassigned for the game.
         */
        // Since you only have a few cores, we do not want to disable most/many of them.
        mask -= 2;
        
        return mask; 
    }
    // 2 cores/threads are left unassigned for CPUs with 6 to 8 cores
    else if (processorCount <= 8)
    {
        /* 6 cores -> 1 << 6 = 64  (100_0000) - 4 = 60 (011_1100) -> Enables cores 2,3,4,5
         * 7 cores -> 1 << 7 = 128 (1000_0000) - 4 = 124 (0111_1100) -> Enables cores 2,3,4,5,6
         * 8 cores -> 1 << 8 = 256 (1_0000_0000) - 4 = 252 (0_1111_1100) -> Enables cores 2,3,4,5,6,7
         * All of them exclude cores 0 and 1, leaving them as unassigned for the game.
         */
        mask -= 4; 

        return mask; 
    }
    
    // If the user's CPU has more than 8 cores, limit it to max amount recommended (8 cores)
    return MAX_ALLOWED_MASK;
}

int desiredAffinity = CalculateAffinityMask();
        
// Keep track if we've already modified a process
// Default -1 to prevent any errors (or to cause some on purpose ¯\_(ツ)_/¯ /jk )
var lastSetPID = -1;

Console.WriteLine("Monitoring for the game process...");

// Keeps looping as long as the app runs, looking for a specific process (in this case, Rome2.exe)
while (true)
{
    var process = Process.GetProcessesByName(GAME_PROCESS_NAME).FirstOrDefault();

    // If we find the process we are looking for
    if (process != null)
    {
        // Only set affinity if we haven't processed this PID before
        if (process.Id != lastSetPID)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nFound {GAME_PROCESS_NAME}.exe (PID: {process.Id})");

            try
            {
                // Set the ProcessorAffinity as requested
                // We give a decimal integer and use its bit representation to indicate cores.
                process.ProcessorAffinity = desiredAffinity;
                lastSetPID = process.Id;
                Console.WriteLine($"CPU affinity is set for {GAME_PROCESS_NAME} (PID: {process.Id}) at {DateTime.Now}");
                Console.ResetColor();

                // Job done: no need to keep the window open or keep monitoring.
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                lastSetPID = -1;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error setting affinity: {ex.Message}");
                Console.ResetColor();
            }
        }
    }

    // There is no process with that name currently running
    else
    {
        /* No game process found but lastSetPID is set.
         * Meaning, game was running but the user did shutdown the game. 
         * Reset lastSetPID to default
         */
        if (lastSetPID != -1)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Game process {GAME_PROCESS_NAME} (PID: {lastSetPID}) is no longer running");

            lastSetPID = -1;
            
            Console.WriteLine("\nPress Ctrl+C to exit\n");
            Console.ResetColor();
            Console.WriteLine("Monitoring for the game process...");
        }
    }

    // Checks once every minute if the target game/process/app is running 
    await Task.Delay(CHECK_DELAY_MS);
}