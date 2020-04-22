# Unregister event javascript action

This action prints "Unregister event" + the name of a event to the log.

## Inputs

### `event-to-unregister`

**Required** The name of the event to unregister. Default `"1000"`.

## Outputs

### `time`

The time we greeted you.

## Example usage

uses: actions/UnRegisterBCEvent-javascript-action@v1
with:
  evnet-to-unregister: '2000'