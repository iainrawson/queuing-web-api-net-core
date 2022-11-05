# Queuing Web API in .NET Core

This provides a rest endpoint which can be used to queue items to be processed in the background.  The queue has a finite limit, and will prevent additional items from being queued until the queue has capacity to accept the new item (to provide back pressure).