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

Find the duration of a video from its URL. This function can throw several
exceptions if the process fails, but it can also return null if the process
succeeds and the duration simply can't be determined.

Although fractional seconds are sometimes included, some formats are only accurate to the nearest second.

Parameters:

* **url**: A public URL pointing to an MP4, HLS, YouTube, or Vimeo video
* **youTubeKey**: A YouTube Data API v3 key. If not provided, all YouTube URLs will return a duration of null, as if they were not recognized.

Returns the duration, or null if the duration could not be determined.

    async Task<TimeSpan?> Duration.GetAsync(string provider, string id, string youTubeKey = null)

Find the duration of a video from its provider and ID. This function can throw
several exceptions if the process fails, but it can also return null if the
duration can't be determined or if the provider is unrecognized.

Parameters:

* **provider**: The provider as given by [jsVideoUrlParser](https://github.com/Zod-/jsVideoUrlParser). Currently supported providers are "youtube" and "vimeo".
* **id**: The ID of the video.
* **youTubeKey**: A YouTube Data API v3 key. If not provided, all YouTube URLs will return a duration of null, as if they were not recognized.

Returns the duration, or null if the duration could not be determined.

Other functions are available - see the source code for more details.

## Azure Functions API

The source code to DurationHelper includes an Azure Functions project. If the application you're working on doesn't use .NET, you could deploy this project to your Azure account (using a Consumption plan) and access it over HTTP.

See FunctionApp/README.md for more information.
