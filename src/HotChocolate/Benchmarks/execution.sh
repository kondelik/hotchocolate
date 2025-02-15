#!/bin/sh

rm -rf BenchmarkDotNet.Artifacts
rm -rf src/Benchmarks.Execution/bin
rm -rf src/Benchmarks.Execution/obj

dotnet run --project src/Benchmarks.Execution/ -c release --filter HotChocolate.Benchmarks.IntrospectionBenchmarks*

rm -rf src/Benchmarks.Execution/bin
rm -rf src/Benchmarks.Execution/obj
