import { HubConnectionBuilder } from "@microsoft/signalr"
import { WaitingRoom } from "./components/WaitingRoom"
import { useState } from "react";
import { Chat } from "./components/Chat";

function App() {
  const [connection, setConnection] = useState(null);
  const [chatRoom, setChatRoom] = useState(null);
  const [messages, setMessages] = useState([]);

  const joinChat = async (userName, chatRoom) =>  {
    const initConnection = new HubConnectionBuilder()
      .withUrl("https://localhost:7062/chat")
      .withAutomaticReconnect()
      .build();

      initConnection.on("ReceiveMessage", (userName, message) => {
      console.log(userName);
      console.log(message);
    })

    try{
      await initConnection.start();
      await initConnection.invoke("JoinChat", {userName, chatRoom});

      setConnection(initConnection);
    }
    catch (error) {
      console.log(error);
    }
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-100">
      {connection ? <Chat messages={messages} chatRoom={chatRoom}/> : <WaitingRoom joinChat={joinChat}/>}
    </div>
  )
}

export default App
