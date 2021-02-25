mergeInto(LibraryManager.library, {
    getMicInput: function() {
        console.log("Loaded JS successfully");
        input = voiceControlHandler();
        // Handle actual voice recognition outside of unity for ease
        // Makes adding/changing libraries easier
        // Also may make it easier to use wasm files rather than having unity package them
        return input;
    }
});