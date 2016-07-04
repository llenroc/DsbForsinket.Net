Retired project, I moved to a self-hosted service implemented in Akka instead - [dsb-forsinket-akka](https://github.com/omarcin/dsb-forsinket-akka).

Back-end service for the android application which allows the user to subscribe to push notifications about delays in public transportation and quickly check the departures board of selected stations.

The android application is avaliable in my [dsb-forsinket-android](https://github.com/omarcin/dsb-forsinket-android) repository.

The app uses [Rejseplannen's API](http://rejseplannen.dk) to get the information about the departures and delays.
Azure Notification Hub is used for sending push notifications. Azure Scheduler periodically triggers Web Jobs to check for delays and send out notifications.

The MIT License (MIT)

Copyright (c) 2016 Marcin Oczeretko

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.