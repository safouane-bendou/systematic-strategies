
import subprocess
import argparse
import sys
from pathlib import Path
from xml.dom.expatbuilder import Skipper
from zipfile import BadZipFile, ZipFile
import os
import shutil
from subprocess import STDOUT
import time

### Creation of folder hierarchy containing the test results
def MakeBacktestResultStructure(testsFolderPath, outFolderPath):
    propOutputFolder = Path(outFolderPath).joinpath('Backtest')
    Path.mkdir(propOutputFolder)
    for filename in testsFolderPath.iterdir():
        if filename.is_dir():
            Path.mkdir(propOutputFolder.joinpath(filename.name))
    return testsFolderPath, propOutputFolder

### Extraction and building of a solution
# Principle: give as an input a zipped folder containing the solution to compile, along with a destination folder where the solution will be extracted. 
# The script compiles the entire unzipped solution.
def ExtractBuildSolution(zippedFolderPath, destinationFolderName):
    try:
        unzippedFolder = zippedFolderPath.stem
        if not zippedFolderPath.exists():
            print(f'No zippped folder {zippedFolderPath.name} available on path')
            sys.exit(1)
        print(f"Unzipping zipped folder at {zippedFolderPath} into {destinationFolderName}")
        with ZipFile(zippedFolderPath, 'r') as zipObj:
            zipObj.extractall(destinationFolderName)
            print('Done with extraction')
        newFolder = Path(destinationFolderName).joinpath(unzippedFolder)
        if not Path(newFolder).exists():
            print(f'Wrong extracted folder name, expected {newFolder.stem}')
            sys.exit(1)
        os.chdir(newFolder)
        print(f'Building solution in folder {unzippedFolder}')
        subprocess.run("dotnet build /nowarn:msb3246,msb3270 /p:Platform=x64 -v q", check=True)
        print(f"Done building solution in folder {unzippedFolder}")
        return True, newFolder
    except BadZipFile:
        print(f'Invalid zipped folder: {zippedFolderPath.name}')
        return False, None
    except subprocess.CalledProcessError as e:
        print(f'Error when building solution in folder {unzippedFolder}')
        return False, None
    except subprocess.TimeoutExpired:
        print('Timeout')
        return False, None
    except SystemExit:
        print('Exiting...')
        return False, None

### Execution of a backtest from an application console
def BacktestConsoleTests(solutionFolder, testPropFolder, outPropFolder):
    try:
        print('Running tests on console')        
        binPath = 'BacktestConsole/bin/x64/Debug/net6.0'
        solutionFolderPath = Path(solutionFolder)
        solutionName = solutionFolderPath.name
        solutionPropFolder = solutionFolderPath.joinpath(binPath)
        if not Path(solutionPropFolder).exists():
            print(f'Wrong path {solutionPropFolder}')
            sys.exit(1)
        for filename in testPropFolder.iterdir():
            if filename.is_dir():
                RunBacktestWith(solutionPropFolder, filename, outPropFolder.joinpath(filename.name), solutionName)
    except SystemExit:
        print('Exiting...')

def RunBacktestWith(propagationFolder, testFolder, outFolder, solutionName):
    print(f'Running test in folder {testFolder}')
    os.chdir(propagationFolder)  
    for filename in os.listdir(testFolder):
        if filename.endswith('.csv') or filename.endswith('.json'):
            shutil.copy(testFolder.joinpath(filename), propagationFolder)
    resultFile = str(outFolder.joinpath(solutionName))
    subprocess.run(f"BacktestConsole.exe test_params.json mkt_data.csv {resultFile}_pf_vals.csv {resultFile}_th_vals.csv", check=True)        


### Main orchestrator for tests  
def run_tests():
    parser = argparse.ArgumentParser()
    parser.add_argument("--sln", help="Path to the folder containing the zipped solution.", type=str)
    parser.add_argument("--tests", help="Path to folder containing tests to run", type=str)
    parser.add_argument("--build", help="Path to folder where the solution will be extracted and built")
    parser.add_argument("--out", help="Path to folder where the results will be stored")
    parser.add_argument("--force", action='store_true', help="Force output folder to be erased if it already exists. Default: false")
    args = parser.parse_args()
    if not args.sln:
        print('Missing "--sln" parameter')
        sys.exit(1)
    if not args.tests:
        print('Missing "--tests" parameter')
        sys.exit(1)
    if not args.build:
        print('Missing "--build" parameter')
        sys.exit(1)
    if not args.out:
        print('Missing "--out" parameter')
        sys.exit(1)
    else:
        zippedFolderPath = Path(args.sln)
        testsFolderPath = Path(args.tests)
        buildFolderPath = Path(args.build)
        outFolderPath = Path(args.out)
        if  Path(outFolderPath).exists():
            if args.force:
                shutil.rmtree(outFolderPath)
            else:
                print('Error: output folder already exists')
                sys.exit(1)
        Path.mkdir(outFolderPath)
        testPropFolder, outPropFolder = MakeBacktestResultStructure(testsFolderPath, outFolderPath)
        for filename in zippedFolderPath.iterdir():
            if filename.suffix == '.zip':
                ok, solutionFolder = ExtractBuildSolution(zippedFolderPath.joinpath(filename), buildFolderPath)
                if ok:
                    BacktestConsoleTests(solutionFolder, testPropFolder, outPropFolder)                    
if __name__ == "__main__":
    run_tests()

