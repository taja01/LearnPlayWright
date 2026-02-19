// Enable parallel execution at assembly level
[assembly: Parallelizable(ParallelScope.All)]

// Set the number of parallel workers (optional - defaults to CPU count)
[assembly: LevelOfParallelism(6)]