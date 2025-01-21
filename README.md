# CLI App

This is a command-line interface (CLI) application written in C# which will download a file from a given URL using HTTP, parse it and write to standard output result of one of two commands:

`count` or `max-age`.

## `count` command

`app.exe count <url> [--age-gt <age> | --age-lt <age>]`

This command counts number of participants satisifing condition and writes it to standard output.

### Options

- `--age-gt <age>` - counts participants where age is greater than `<age>`, where `<age>` is an integer
- `--age-lt <age>` - counts participants where age is less than `<age>`, where `<age>` is an integer

### Example

`app.exe count https://tomtar00.github.io/cli-app/data.json`
should write to standard output: `5`

`task.exe count https://tomtar00.github.io/cli-app/data.json --age-gt 22`
should write to standard output: `2`

## `max-age` command

`task.exe max-age <url>`

This command writes a maximum age of a participant to standard output.

### Example

`task.exe max-age https://tomtar00.github.io/cli-app/data.json`
should write to standard output: `24`

## Download & parse file

The file contains a list of participants in three different formats: JSON, CSV, and ZIP archive with a CSV file. Participant data model consists of following properties:

- id: integer,
- age: integer,
- name: string,
- email: string,
- workStart: date with time,
- workEnd: date with time.

File type is determined based on content type. It can be JSON, CSV, or ZIP archive with CSV inside. Examples of files are accessible from the following URLs:

- https://tomtar00.github.io/cli-app/data.json
- https://tomtar00.github.io/cli-app/data.csv
- https://tomtar00.github.io/cli-app/data.zip

## How to build

To build the project use the standard `dotnet build` command.
