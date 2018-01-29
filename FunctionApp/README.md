# DurationHelper FunctionApp

If the application you're working on doesn't use .NET, you could deploy this project to your Azure account (using a Consumption plan) and access it over HTTP.

[Try it](https://www.lakora.us/DurationHelper/api.html)

### get-duration

    /api/get-duration?url={url}

Tries to get the duration of a video. Although fractional seconds are sometimes included, some formats are only accurate to the nearest second.

**HTTP response codes**

* **200**: Duration found
* **204**: Duration unknown
* **400**: Missing or invalid URL
* **500**: An unknown error occurred
* **502**: A URL could not be loaded, or a YouTube API error occurred

**Example request**

    GET /api/get-duration?url=https%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv%3DyrDO1zGqJXg

**Example response**

    178.0

### parse-youtube-url

    /api/parse-youtube-url?url={url}

Extracts the ID and a few parameters from a YouTube URL.

**HTTP response codes**

* **200**: Duration found
* **400**: Missing or invalid URL
* **500**: An unknown error occurred

**Example request**

    GET /api/parse-youtube-url?url=https%3A%2F%2Fwww.youtube.com%2Fwatch%3Fv%3DyrDO1zGqJXg%26start%3D10%26end%3D30%26autoplay%3D1

**Example response**

    {"id":"yrDO1zGqJXg","start":10,"end":30,"autoplay":true}
