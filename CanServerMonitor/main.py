from fastapi import FastAPI

app = FastAPI()

@app.post("/api/can")
async def receive_can_data(can_data: dict):
    print("CAN Data received: ", can_data)
    return {"message": "CAN Data received successfully"}
