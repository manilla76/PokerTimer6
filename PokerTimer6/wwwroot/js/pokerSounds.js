window.pokerSounds = {
    playLevelEnd: () => {
        const audio = document.getElementById('levelEndAlert');
        if (audio) {
            audio.currentTime = 0;                   // rewind to start
            audio.play().catch(err => {
                console.warn('Audio play blocked or failed:', err);
            });
        }
    }
};