mergeInto(LibraryManager.library, {
    startRecogniser: function() {
        console.log("Loaded Internal JS successfully");
        startRecording();
        //Starting record
        //Separate frontend function will handle return
    }
});