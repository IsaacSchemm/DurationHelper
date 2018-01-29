# DurationHelper

https://www.nuget.org/packages/DurationHelper

DurationHelper is a .NET Standard library that does its best to determine the duration of a video when given its URL.

Supported formats:

* MP4
* HLS video-on-demand (VOD)
* YouTube
* Vimeo

To extract the duration of an MP4 file, the moov atom, which contains duration info, must be in the first 256 bytes of the file.

DurationHelper can also extract the ID and start, end, and autoplay parameters from a YouTube URL. Full (youtube.com) and short (youtu.be) URLs are supported.

## Usage

    async Task<TimeSpan?> Duration.GetAsync(Uri uri, string youTubeKey = null)

Find the duration of a video from its URL. This function can throw several exceptions if the process fails, but it can also return null if the process succeeds and the duration simply can't be determined.

Although fractional seconds are sometimes included, some formats are only accurate to the nearest second.

Parameters:

* **url**: A public URL pointing to an MP4, HLS, YouTube, or Vimeo video
* **youTubeKey**: A YouTube Data API v3 key. If not provided, all YouTube URLs will return a duration of null, as if they were not recognized.

Returns the duration, or null if the duration could not be determined.

    YouTubeUrlInfo ParseUrl(Uri url)

Extract a YouTube ID and a few parameters from a YouTube URL.

Parameters:

* **url**: The YouTube URL (youtube.com or youtu.be)

Returns information extracted from the URL:

* **id**: The extracted YouTube ID.
* **start**: The start time, in seconds, specified in the URL (if any.)
* **end**: The end time, in seconds, specified in the URL (if any.)
* **autoplay**: Whether the autoplay=1 parameter is present in the URL.

Other functions are available - see the source code for more details.

## Azure Functions API

The source code to DurationHelper includes an Azure Functions project. If the application you're working on doesn't use .NET, you could deploy this project to your Azure account (using a Consumption plan) and access it over HTTP.

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
