# DurationHelper

https://www.nuget.org/packages/DurationHelper

DurationHelper is a .NET Standard library that does its best to determine the duration of a video when given its URL.

**This library is now deprecated; version 3.x is a wrapper around [DurationFinder](https://www.nuget.org/packages/ISchemm.DurationFinder).**

## Usage

    async Task<TimeSpan?> Duration.GetAsync(Uri uri, string youTubeKey = null, ITwitchCredentials twitchCredentials = null)

Find the duration of a video from its URL. This function can throw several
exceptions if the process fails, but it can also return null if the process
succeeds and the duration simply can't be determined.

Although fractional seconds are sometimes included, some formats are only
accurate to the nearest second.

Parameters:

* **url**: A public URL pointing to an MP4, HLS, YouTube, Dailymotion, Twitch, or Vimeo video, or a SoundCloud post
* **youTubeKey**: Ignored. YouTube lookups no longer use the API.
* **twitchCredentials**: Ignored. Twitch lookups no longer use the API.

Returns the duration, or null if the duration could not be determined.

    async Task<TimeSpan?> Duration.GetAsync(string provider, string id, string youTubeKey = null, ITwitchCredentials twitchCredentials = null)

Find the duration of a video from its provider and ID. This function can throw
several exceptions if the process fails, but it can also return null if the
duration can't be determined or if the provider is unrecognized.

Parameters:

* **provider**: The provider as given by [jsVideoUrlParser](https://github.com/Zod-/jsVideoUrlParser). Currently supported providers are "youtube", "dailymotion", "twitch", and "vimeo".
* **id**: The ID of the video.
* **youTubeKey**: Ignored. YouTube lookups no longer use the API.
* **twitchCredentials**: Ignored. Twitch lookups no longer use the API.

Returns the duration, or null if the duration could not be determined.

Other functions are available - see the source code for more details.
