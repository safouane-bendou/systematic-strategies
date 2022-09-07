The script that will be used for the evaluation is similar to 'generate-backtest-results.py'. This script takes the following arguments (*** all paths are absolute ***):
- sln: the path to the folder containing your zipped solution
- tests: the path to the folder contining the tests to be run (the path to 'TestCases', including the latter for the provided example)
- build: the path to the folder where the solutions will be extracted and built
- out: the path to the folder where the outputs produced by the code will be stored
- force: a boolean argument stating whether the files in the output folder are overwritten or not

Command on Windows:
python.exe generate-bactest-results.py --sln=<abs-path-to-networks> --tests=<abs-path-to-tests> --out=<abs-path-to-produced-outputs> --build=<abs-path-to-built-code> --force