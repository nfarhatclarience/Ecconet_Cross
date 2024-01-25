#read a text file and find the unique ids
import os

ids = []
# read the file in the same directory called canFrameID.txt
with open(os.path.join(os.path.dirname(__file__), 'canFrameID.txt'), 'r') as f:
    # read the file line by line
    for line in f:
        # split the line by space
        line = line.split()
        # print the first element of the line
        ids.append(line[0])
# print the unique ids by converting the list to a set  then set back to a list
# print the length of the list
print(len(ids))
print(len(list(set(ids))))