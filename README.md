# Goals

## Capabilities
- [x] Allow tracking times
- [ ] Allow tracking what was done
    - Basically answer the question "What did I do in the time from ... to ...?"
- [ ] Allow to see how much time/until when I have to work to fulfill my 8 hours on the
    current day or how much I need to work given the current accumulated overtime
- [ ] Allow to verify if there is something suspicious with the data
    - [ ] More than one break a day (might be caused by forgetting to start tracking)
    - [ ] Less than 8 hours worked
        - Maybe only if it there was not enough overtime available to justify?
- [ ] Provide summary that allows to see
    - [ ] For a time frame, from when to when did I work with how much break time
    - [ ] What is my current accumulated overtime
- [ ] Allow adding notes with timestamps

## Technology
- [ ] Split into multiple projects
    - [ ] Frontend - I might want to support a different interface, especially for
    evaluation
    - [ ] Core
- [ ] Allow exchangable storage options as I am not sure if I am satisfied with the
    capabilities of CSV
- [ ] Add dependency injection
- [ ] Add tests

# Usage

## Track

```sh
track <filePath>
```

This command either starts tracking a task or, if it's already tracking, ends it.

If it starts tracking, it will query for
- project
- ticket
- comment (i.e. a description)

## Evaluation

```sh
eval <filePath> <months>
```

This command will give a summary of the last months (including the current one). If no
amount of months is specified, it will evaluate the current and previous month.

### Results with specified months
For each month it will show
- time tracked
- number of days worked

### Results without specified months
This will give an overview of
- total time worked in this month and the previous one
- total time worked for each of these months
- days worked for the current and previous month
- accumulated overtime
- a breakdown per project:
    - time worked today
    - time worked this month
    - time worked in the previous month

## Listing data
```sh
list <filePath>
```

Lists all data in the given file in a table

## Checking data
```sh
check <filePath>
```

Lists a summary of each day consisting of
- date
- time worked
- overtime accrued (if any)
- the number of breaks

If something looks odd, it will be highlighted
