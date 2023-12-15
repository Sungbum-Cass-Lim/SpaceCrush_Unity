Module.WebEventListener = function (e) {
  //console.log("WebEventListener", e.origin, Data.ORIGIN);
  if (Data.ORIGIN.indexOf(e.origin) === -1 && Data.ORIGIN.indexOf("") === -1) 
  {
    return;
  }

  let message = null;

  // json 형식이 아니면 안됨
  try 
  {
    message = JSON.parse(e.data);
  } 
  catch (error) 
  {
    return;
  }
  console.log(message);

  switch (message.message) 
  {
    case "token":
    
    case "onToken":
      unityInstance.SendMessage(
        "WebNetworkMgr",
        "OnRequestToken",
        JSON.stringify(message.data)
      );
      break;

    case "onTarget":
      unityInstance.SendMessage(
        "WebNetworkMgr",
        "OnRequestTarget",
        JSON.stringify(message.data)
      );
      break;

    case "onRestart":
      console.log("Restart Called");
      unityInstance.SendMessage(
        "WebNetworkMgr",
        "OnRequestRestart",
      );
      break;  

    case "pause":
      break;

    case "mute":
      unityInstance.SendMessage(
        "WebNetworkMgr",
        "OnRequestMute",
        JSON.stringify(message.data)
      );
      break;
  }
};

Module.SendPostMessage = function (data) {
  const stringData = JSON.stringify(data);
  console.log("[Send Message] ", stringData);
  window.parent.postMessage(stringData, "*");
};

// postMessage object return
Module.GetPostMessage = function (message, data) {
  if (data === null || data === undefined || data === "") data = {};
  return { message: message, data: data };
};
