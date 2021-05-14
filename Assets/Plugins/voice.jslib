mergeInto(LibraryManager.library, {
    //Underscore and dependency based on forum answer from Unity employee.
    //https://forum.unity.com/threads/browser-scripting-and-function-calling.477716/
    //Should be solid (should also be explained in the documentation though imo)
    //Second forum post:
    //https://forum.unity.com/threads/call-javascript-function-in-index-html-from-webgl.625480/
    startRecogniser__deps: ['startRecording'],
    startRecogniser: function() {
        console.log("Loaded Internal JS successfully");
        startRecording();
        //Starting record
        //Separate frontend function will handle return
    },
    setupMicrophoneUnity__deps: ['setupVoiceDetection'],
    setupMicrophoneUnity: function() {
        setUpVoiceDetection();
    },
    setupVoiceChatUnity__deps: ['setupVoiceChat'],
    setupVoiceChatUnity: function(roomName, role) {
        setupVoiceChat(roomName, role);
    },
    disableVoiceChatUnity__deps: ['disableVoiceChat'],
    disableVoiceChatUnity: function() {
        disableVoiceChat();
    },
    reenableVoiceChatUnity__deps: ['reenableVoiceChat'],
    reenableVoiceChatUnity: function() {
        reenableVoiceChat();
    }
});