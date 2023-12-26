function attachMXNotifier () {
    var notifier = {
        notifierCallbacks: {},
        on: function (eventName, func, context) {
            this.notifierCallbacks[eventName] = context ? func.bind(context) : func
        },
        emit: function (eventName, args) {
            let func
            if (eventName === 'commonEventInterface') {
                func = this.notifierCallbacks[args]
            } else {
                func = this.notifierCallbacks[eventName]
            }
            if (typeof func === "function") {
                func(args)
            }
        }
    }

    if (typeof window !== 'undefined') {
        if (typeof window.cc === 'undefined') {
            window.cc = {
                game: notifier
            }
        }
        window.MX_Notifier = notifier
    }
}

function attachIOSPolyfills () {
    if (window.webkit && window.webkit.messageHandlers && !window.gameManager) {
        window.gameManager = {}
        window.gameManager.onGameStart = function () { 
            window.webkit.messageHandlers.onGameStart.postMessage({})
         }

        window.gameManager.onError = function (errStr) {
            window.webkit.messageHandlers.onError.postMessage({ errStr: errStr })
        }

        window.gameManager.onGameInit = function () {
            if (typeof window.config === 'string') {
                try {
                    const configObj = JSON.parse(window.config)
                    if (window.restartHighScore) {
                        configObj.highestScore = window.restartHighScore
                    }
                    return JSON.stringify(configObj)
                } catch(e) { return {} }
            } else if (typeof window.config === 'object') {
                if (window.restartHighScore) {
                    window.config.highestScore = window.restartHighScore
                }
                return JSON.stringify(window.config)
            }
            return '{}'
        }

        window.gameManager.onTrack = function (eventName, data) {
            window.webkit.messageHandlers.onTrack.postMessage({ eventName: eventName, data: data })
        }

        window.gameManager.onGameOver = function (data) {
            window.webkit.messageHandlers.onGameOver.postMessage({ data: data })
        }

        window.gameManager.onCheckRewardedVideoAds = function (notificationName, argObj) {
            window.webkit.messageHandlers.onCheckRewardedVideoAds.postMessage({ notificationName: notificationName, argObj: argObj })
        }

        window.gameManager.onShowRewardedVideoAds = function (notificationName, argObj) {
            window.webkit.messageHandlers.onShowRewardedVideoAds.postMessage({ notificationName: notificationName, argObj: argObj })
        }

        window.gameManager.getGameSettings = window.gameManager.onGameInit
    }
}

function INIT_MX () {}

INIT_MX.prototype.restartGame = function (highScore) {
    window.restartHighScore = highScore
    if (typeof window._MX_.restartGameCallback === 'function') {
        window._MX_.restartGameCallback()
        return true
    }
    return false
}
INIT_MX.prototype.calcInsetPadding = function () {
    var root = document.documentElement;
    // add CSS variables so we can read them back
    root.style.setProperty('--notch-top', 'env(safe-area-inset-top)');
    root.style.setProperty('--notch-right', 'env(safe-area-inset-right)');
    root.style.setProperty('--notch-bottom', 'env(safe-area-inset-bottom)');
    root.style.setProperty('--notch-left', 'env(safe-area-inset-left)');
    // get runtime styles
    var style = window.getComputedStyle(root);
    // read env values back and check if we have any values
    var insetPadArr = [
        parseInt(style.getPropertyValue('--notch-top').replace('px', '').trim() || '0', 10),
        parseInt(style.getPropertyValue('--notch-right').replace('px', '').trim() || '0', 10),
        parseInt(style.getPropertyValue('--notch-bottom').replace('px', '').trim() || '0', 10),
        parseInt(style.getPropertyValue('--notch-left').replace('px', '').trim() || '0', 10)
    ]
    return {
        top: insetPadArr[0],
        right: insetPadArr[1],
        bottom: insetPadArr[2],
        left: insetPadArr[3]
    }
}

function onload () {
    console.log("IM ON LOAD");
    attachIOSPolyfills()
    attachMXNotifier()
    if (typeof window !== 'undefined') {
        window._MX_ = new INIT_MX()
    }
}

onload()