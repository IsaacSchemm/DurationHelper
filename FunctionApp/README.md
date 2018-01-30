# DurationHelper FunctionApp

If the application you're working on doesn't use .NET, you could deploy this project to your Azure account (using a Consumption plan) and access it over HTTP.

[Try it](https://www.lakora.us/DurationHelper/api.html)

### get-duration

    /api/get-duration?url={url}

Tries to get the duration of a video. Although fractional seconds are sometimes included, some formats are only accurate to the nearest second.

Parameters:

* **url**: A public URL pointing to an MP4, HLS, YouTube, or Vimeo video.

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

### get-duration-by-id

    /api/get-duration-by-id?provider={provider}&id={id}

Tries to get the duration of a video hosted on a given provider.

Parameters:

* **provider**: The provider as given by [jsVideoUrlParser](https://github.com/Zod-/jsVideoUrlParser). Currently supported providers are "youtube" and "vimeo".
* **id**: The ID of the video.

**HTTP response codes**

* **200**: Duration found
* **204**: Duration unknown
* **400**: Missing or invalid parameters
* **500**: An unknown error occurred
* **502**: A URL could not be loaded, or a YouTube API error occurred

**Example request**

    GET /api/get-duration-by-id?id=237205665&provider=vimeo

**Example response**

    114.0
