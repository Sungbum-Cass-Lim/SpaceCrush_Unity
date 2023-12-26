const gameManager = {
    onGameInit: function () {
        const config = {
            roomId: 'roomId',
            gameId: 'gameId',
            userId: 'userId',
            highestScore: 0
        }
        const configStr = JSON.stringify(config)
        return configStr
    },
    onGameStart: function () {
        if (this.errorHandled) { return }
        this.gameStarted = true
        setOVerlayText("Removing Loading Screen", "gameManager.onGameStart implementation working fine")
        setTimeout(() => {
            const fullOverlay = document.getElementById('full-overlay')
            fullOverlay.classList.add('close')
        }, 1000)
    },
    onTrack: function (eventName, data) {
        if (this.errorHandled) { return }
        if (!this.gameStarted) {
            this.errorHandled = true
            setOVerlayText("Error", "gameManager.onTrack called before gameManager.onGameStart")
            const fullOverlay = document.getElementById('full-overlay')
            fullOverlay.classList.remove('close')
            return
        }
        console.log("Tracking", eventName, data)
    },
    onError: function (err) {
        if (this.errorHandled) { return }
        if (!this.gameStarted) {
            this.errorHandled = true
            setOVerlayText("Error", "gameManager.onError called before gameManager.onGameStart")
            const fullOverlay = document.getElementById('full-overlay')
            fullOverlay.classList.remove('close')
            return
        }
        console.log("Error", err)
    },
    onCheckRewardedVideoAds: function (event) {
        if (this.errorHandled) { return }
        if (!this.gameStarted) {
            this.errorHandled = true
            setOVerlayText("Error", "gameManager.onCheckRewardedVideoAds called before gameManager.onGameStart")
            const fullOverlay = document.getElementById('full-overlay')
            fullOverlay.classList.remove('close')
            return
        }
        this.checkRewardedVideoAdsEvent = event
        const partialOverlayHeader = document.getElementById('partial-overlay-header')
        partialOverlayHeader.innerHTML = "Event - " + event
        const partialOverlay = document.getElementById('partial-overlay')
        partialOverlay.classList.add('open')
    },
    _adExistsCallback: function () {
        const partialOverlay = document.getElementById('partial-overlay')
        partialOverlay.classList.remove('open')

        window.cc.game.emit(this.checkRewardedVideoAdsEvent, { status: 0 })

    },
    _adNotExistCallback: function () {
        const partialOverlay = document.getElementById('partial-overlay')
        partialOverlay.classList.remove('open')

        window.cc.game.emit(this.checkRewardedVideoAdsEvent, { status: 1 })
    },
    onShowRewardedVideoAds: function (event) {
        if (this.errorHandled) { return }
        if (!this.gameStarted) {
            
            this.errorHandled = true
            setOVerlayText("Error", "gameManager.onShowRewardedVideoAds called before gameManager.onGameStart")
            const fullOverlay = document.getElementById('full-overlay')
            fullOverlay.classList.remove('close')
            return
        }
        this.onShowRewardedVideoAdsEvent = event
        setOVerlayText("AD Screen", "gameManager.onShowRewardedVideoAds called")
        const fullOverlay = document.getElementById('full-overlay')
        fullOverlay.classList.add('ad-screen')
        fullOverlay.classList.remove('close')
    },
    _adFullySeen: function () {
        const fullOverlay = document.getElementById('full-overlay')
        fullOverlay.classList.add('close')
        fullOverlay.classList.remove('ad-screen')

        window.cc.game.emit(this.onShowRewardedVideoAdsEvent, { status: 0 })
    },
    _adFullyNotSeen: function () {
        const fullOverlay = document.getElementById('full-overlay')
        fullOverlay.classList.add('close')
        fullOverlay.classList.remove('ad-screen')

        window.cc.game.emit(this.onShowRewardedVideoAdsEvent, { status: 1 })
    },
    onGameOver: function (data) {
        if (this.errorHandled) { return }
        if (!this.gameStarted) {
            this.errorHandled = true
            setOVerlayText("Error", "gameManager.onGameOver called before gameManager.onGameStart")
            const fullOverlay = document.getElementById('full-overlay')
            fullOverlay.classList.remove('close')
            return
        }
        setOVerlayText("Game Over", `gameManager.onGameOver called ${data}`)
        const fullOverlay = document.getElementById('full-overlay')
        fullOverlay.classList.remove('close')
    }
}

function renderFullOverlay () {
    var css = `
        #full-overlay { 
            position: fixed;
            top: 0px; 
            left: 0px; 
            width: 100%; 
            height: 100%; 
            z-index: 100000; 
            background-color: #3C8CF0; 
            transition: 0.5s transform;
        } 
        #full-overlay.close { 
            transform: translateY(100%); 
        } 
        .full-overlay-content { 
            margin: 200px auto; 
            color: #fff; 
            text-align: center; width: 250px; 
            overflow: scroll;
        } 
        #full-overlay-header { 
            font-size: 24px; 
        } 
        #full-overlay-subheader { 
            font-size: 14px; 
            margin: 30px 0; 
        }
        .full-overlay-actionables {
            display: none
        }
        #full-overlay.ad-screen .full-overlay-actionables{
            display: block;
        }
        #partial-overlay { 
            position: fixed; 
            bottom: 0px;
            left: 0px; 
            width: 100%; 
            height: 150px; 
            z-index: 110000; 
            background-color: #0f396e; 
            transform: translateY(100%);
            transition: 0.5s transform; 
        }
        #partial-overlay-header {
            margin: 20px 0;
        }
        #partial-overlay.open { 
            transform: translateY(0); 
        }
        .partial-overlay-content {
            display: block;
        }
        .ad-checker {
            background-color: transparent;
            border: 1px solid #fff;
            color: #fff;
            border-radius: 5px;
            padding: 5px 20px;
            font-size: 16px;
            display: block;
            margin: 10px auto;
            width: 200px;
        }
    `
    var head = document.querySelectorAll('head')[0]
    var style = document.createElement('style')
    head.appendChild(style)
    if (style.styleSheet){
        style.styleSheet.cssText = css;
    } else {
        style.appendChild(document.createTextNode(css));
    }

    const fullOverlay = document.createElement('div')
    fullOverlay.id = 'full-overlay'

    fullOverlay.innerHTML = `
        <div class="full-overlay-content">
            <div id="full-overlay-header"></div>
            <div id="full-overlay-subheader"></div>
            <div class="full-overlay-actionables">
                <div id='ad-seen' class='ad-checker'>Check Fully Seen flow</div>
                <div id='ad-not-seen' class='ad-checker'>Check Not Fully Seen flow</div>
            </div>
        </div>
    `

    const partialOverlay = document.createElement('div')
    partialOverlay.id = 'partial-overlay'

    partialOverlay.innerHTML = `
        <div class="partial-overlay-content">
            <div id="partial-overlay-header"></div>
            <div id='ad-exists' class='ad-checker'>Check Ad Exists flow</div>
            <div id='ad-not-exists' class='ad-checker'>Check Ad Not Exists flow</div>
        </div>
    `
    const body = document.querySelectorAll('body')[0]
    body.appendChild(fullOverlay)
    body.appendChild(partialOverlay)


    const adExists = document.getElementById('ad-exists')
    adExists.addEventListener('click', gameManager._adExistsCallback.bind(gameManager))

    const adNotExists = document.getElementById('ad-not-exists')
    adNotExists.addEventListener('click', gameManager._adNotExistCallback.bind(gameManager))

    const adSeen = document.getElementById('ad-seen')
    adSeen.addEventListener('click', gameManager._adFullySeen.bind(gameManager))

    const adNotSeen = document.getElementById('ad-not-seen')
    adNotSeen.addEventListener('click', gameManager._adFullyNotSeen.bind(gameManager))
}

function adExistsCallback () {
    cc.game.emit()
}

function setOVerlayText(header, subHeader) {
    document.getElementById('full-overlay-header').innerHTML = header
    document.getElementById('full-overlay-subheader').innerHTML = subHeader
}

function onGameLoad () {
    renderFullOverlay() 
    setOVerlayText("Loading Screen", "Will be removed when gameManager.onGameStart() is called !")
    // gameManager.onGameInit();
    // gameManager.onGameStart();
    // gameManager.onTrack('blaa')
}

window.addEventListener('DOMContentLoaded', onGameLoad);

window.gameManager = gameManager