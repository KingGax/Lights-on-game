mergeInto(LibraryManager.library, {
    GetMicInput: function() {
        console.log("Loaded JS successfully");
        input = frontendGetMicInput();
        // Handle actual voice recognition outside of unity for ease
        // Makes adding/changing libraries easier
        // Also may make it easier to use wasm files rather than having unity package them
    }
});