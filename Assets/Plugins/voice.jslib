mergeInto(LibraryManager.library, {
    //Underscore and dependency based on forum answer from Unity employee.
    //https://forum.unity.com/threads/browser-scripting-and-function-calling.477716/
    //Should be solid (should also be explained in the documentation though imo)
    startRecogniser__deps: ['startRecording'],
    startRecogniser: function() {
        console.log("Loaded Internal JS successfully");
        _startRecording();
        //Starting record
        //Separate frontend function will handle return
    }
});