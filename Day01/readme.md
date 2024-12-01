# Advent of Code 2024 - Day 1 Solution
This repository contains a solution for Day 1 of the Advent of Code 2024. The problem involves processing pairs of integers and performing calculations based on their relationships to solve two parts.

## Solution Description
The program reads pairs of integers from the input, processes them, and calculates the following:

Part 1: Calculates the sum of the absolute differences between corresponding integers in the left and right lists after sorting them independently.
Part 2: Calculates a similarity score, where each integer in the left list is multiplied by its frequency in the right list.

## Input Format
The input is expected to contain multiple lines, each with a pair of integers separated by a space.


The program outputs two values:

Solution to Part 1: The sum of the absolute differences between paired and sorted integers.
Solution to Part 2: A calculated similarity score based on frequency counts.

## How It Works
### Input Parsing:

Reads lines of input and splits each line into two integers.
Populates the left and right lists with the respective integers.
Updates a frequency dictionary to count occurrences of integers in the right list.

#### Part 1 Calculation:

Sorts the left and right lists independently.
Zips the sorted lists and computes the absolute difference between corresponding elements.
Sums the differences to produce the solution.

#### Part 2 Calculation:

Multiplies each integer in the left list by its frequency count in the right list (if it exists).
Sums the results to produce the solution.