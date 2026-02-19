// Enable parallel execution at assembly level
[assembly: Parallelizable(ParallelScope.Fixtures)]

// Set the number of parallel workers (optional - defaults to CPU count)
[assembly: LevelOfParallelism(4)]