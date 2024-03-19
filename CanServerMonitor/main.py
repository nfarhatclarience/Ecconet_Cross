from fastapi import FastAPI
from uvicorn import run
import threading
import argparse

# take the server address or local host from the user

parser = argparse.ArgumentParser(description='Server address')
parser.add_argument('--server', type=str, help='Server address')
args = parser.parse_args()
server_address = args.server

class LibConfig:
    # ... (Define other constants as needed)
    MATRIX_CAN_ID_SOURCE_ADDRESS_SHIFT = 17
    MATRIX_CAN_ID_ADDRESS_MASK = 0x7F


class Receiver:
    def __init__(self):
        # ... (Initialize other members)
        self.source_address = 0
        self.rx_callback_lock = threading.Lock()  # Example using threading.Lock

    def receive_can_frame(self, id, data):
        frame_type, source_address, destination_address = 0, 0, 0

        # ... (Other code)

        with self.rx_callback_lock:
            # ... (Frame type filter)

            # Source address
            #convert id from string to hex
            id = int(id, 16)
            
            self.source_address = ( id >> LibConfig.MATRIX_CAN_ID_SOURCE_ADDRESS_SHIFT) & LibConfig.MATRIX_CAN_ID_ADDRESS_MASK

            # ... (Other code)
    def print_source_address(self):
        print("Source address: ", self.source_address)
        
app = FastAPI()

@app.post("/api/can")
async def receive_can_data(can_data: dict):
    receiver = Receiver()
    receiver.receive_can_frame(can_data['id'], can_data['data'])
    receiver.print_source_address()
    #print("CAN Data received: ", can_data)
    return {"message": "CAN Data received successfully"}
if __name__ == "__main__":
    if server_address:
        run("main:app", host=server_address, port=8000, reload=True)
    else : #run local host
        run("main:app")