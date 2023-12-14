var jslib = {
  $Data: {
    ORIGIN: [],
  },

  initialize: function (origin) {
    if (UTF8ToString(origin) === "dev") {
      Data.ORIGIN = [
        "https://tournament.dev.playdapp.com",
        "http://localhost:3000",
      ];
    }
     else if (UTF8ToString(origin) === "qa") {
        Data.ORIGIN = [
          "https://tournament.qa.playdapp.com",
          "http://localhost:3000",
        ];
    }
     else if (UTF8ToString(origin) === "prod") {
        Data.ORIGIN = [
          "https://tournament.playdapp.com",
          "https://tournament.staging.playdapp.com",
        ];
    }
    console.log("init", Data.ORIGIN);
    console.log("unityInstance", unityInstance);

    window.addEventListener("message", unityInstance.Module.WebEventListener);
  },

  // jwt token request
  RequestToken: function () {
    unityInstance.Module.SendPostMessage(
      unityInstance.Module.GetPostMessage("requestToken", null)
    );
  },
  
  RequestTarget: function () {
    unityInstance.Module.SendPostMessage(
      unityInstance.Module.GetPostMessage("requestTarget", null)
    );
  },

  Loading: function (show) {
    unityInstance.Module.SendPostMessage(
      unityInstance.Module.GetPostMessage("loading", { show: show })
    );
  },

  RequestMain: function (show) {
    unityInstance.Module.SendPostMessage(
      unityInstance.Module.GetPostMessage("requestMain", null)
    );
  },
  
  SendFrontError: function(data) {
     unityInstance.Module.SendPostMessage(
          unityInstance.Module.GetPostMessage("error", JSON.parse(UTF8ToString(data)))
    );
  },

 SendEndGame: function(_score) {
    unityInstance.Module.SendPostMessage(
      unityInstance.Module.GetPostMessage("sendEndGame", {score:_score})
    );
  },
};

autoAddDeps(jslib, "$Data");
mergeInto(LibraryManager.library, jslib);
