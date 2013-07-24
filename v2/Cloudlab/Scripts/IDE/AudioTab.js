/// <reference path="../jquery-1.7.1-vsdoc.js" />

var Audio = {

    //-- Functions
    Initialize: function () {
        Audio.player = $('audio').get(0);
        $('#Audio_PlayButton').on('click', Audio.Handlers.Button.Play);

        $('audio').on('ended', Audio.Handlers.Events.Ended);
        $('audio').on('pause', Audio.Handlers.Events.Pause);
        $('audio').on('play', Audio.Handlers.Events.Play);
        $('audio').on('timeupdate', Audio.Handlers.Events.TimeUpdate);
    },
    Notify: function (title, text, sticky, time) {
        $.gritter.add({
            title: title,
            text: text,
            sticky: sticky,
            time: time
        });
    },
    Refresh: function () {

    },
    Source: function (dataURI) {
        Audio.player.src = dataURI;
        Audio.player.load();
        return (Audio.player.src = dataURI);
    },
    Play: function () {
        Audio.Handlers.Button.Play();
    },

    //-- Event Handlers
    Handlers: {
        Button: {
            Play: function () {
                if (Audio.player.paused) {
                    Audio.player.play();
                }
                else {
                    Audio.player.pause();
                }
            }
        },
        Events: {
            Ended: function (e) {
                $('#Audio_PlayButton > i').addClass('icon-play').removeClass('icon-pause');
            },
            Play: function (e) {
                $('#Audio_PlayButton > i').removeClass('icon-play').addClass('icon-pause');
            },
            Pause: function (e) {
                $('#Audio_PlayButton > i').addClass('icon-play').removeClass('icon-pause');
            },
            TimeUpdate: function (e) {
                $('#Audio_Progress').css('width', e.target.currentTime / e.target.duration * 100 + '%');
            }
        }
    },

    //-- Settings & Persistence
    Settings: {

    },

    //-- Internal Vars
    player: null
};