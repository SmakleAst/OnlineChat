import { HubConnectionBuilder } from "@microsoft/signalr"
import { WaitingRoom } from "./components/WaitingRoom"


function App() {
  const joinChat = async (userName, chatRoom) =>  {
    const connection = new HubConnectionBuilder()
      .withUrl("https://localhost:7062/chat")
      .withAutomaticReconnect()
      .build();

      try{
        await connection.start();
        await connection.invoke("JoinChat", {userName, chatRoom});
      }
      catch (error) {
        console.log(error);
      }
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-100">
      <WaitingRoom joinChat={joinChat}/>
    </div>
  )
}

export default App
