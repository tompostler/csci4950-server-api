# Tom Postler, 2015-04-15
# The dummy (to create the data)

from data import *

from sys import exit

import json
import requests


# A useful way to fail
def fail(request, msg):
    if request and msg:
        print(msg, request.status_code)
        print(request.content)
    r = requests.delete(URL+"users/"+str(user['id']), headers=HEADERS)
    if r.status_code != 204:
        print("User delete error:", r.status_code)
        print(r.content)
        exit()
    print("\nDeleted user and all accompanying data")
    exit()



def sep():
    print('-'*80)



# Useful constants
URL = "https://msfrizzle.me/api/"
HEADERS = { "Content-Type": "application/json", "Accept": "application/json" }



# Create the user
sep()
r = requests.post(URL+"users", data=json.dumps(user), headers=HEADERS)
if r.status_code != 200:
    print("User post error:", r.status_code)
    print(r.content)
    exit()
user['id'] = r.json()['id']
print("Created user with ID:", user['id'])



# Insert user data into necessary objects
sep()
print("Updating locations and activities with user_id...", end='')
for location in locations:
    location['user_id'] = user['id']
for activity in activities:
    activity['user_id'] = user['id']
print("Done")



# Create the auth
sep()
auth_req = {
    'user_id': user['id'],
    'password': user['password']
    }
r = requests.post(URL+"auth", data=json.dumps(auth_req), headers=HEADERS)
if r.status_code != 200:
    print("Auth post error:", r.status_code)
    print(r.content)
    exit()
user['token'] = r.json()['token']
HEADERS['Authorization'] = user['token']
print("Created auth with token:", user['token'])



# Create some locations
sep()
for location in locations:
    r = requests.post(URL+'locations', data = json.dumps(location), headers=HEADERS)
    if r.status_code != 200:
        fail(r, "Location '"+ location['name'] +"' creation error")
    location['id'] = r.json()['id']
    print("Location '"+ location['name'] +"' created with ID:", location['id'])



# Create a set of activities
sep()
for activity in activities:
    r = requests.post(URL+'activities', data = json.dumps(activity), headers=HEADERS)
    if r.status_code != 200:
        fail(r, "Activity '"+ activity['name'] +"' creation error")
    activity['id'] = r.json()['id']
    print("Activity '"+ activity['name'] +"' created with ID:", activity['id'])



# Update the activity units appropriately
sep()
print("Updating activity units with activity and location IDs...", end='')
UpdateActivityUnits()
print("Done")



# Create a set of activity units
sep()
for activityunit in activityunits:
    r = requests.post(URL+'activityunits', data = json.dumps(activityunit), headers=HEADERS)
    if r.status_code != 200:
        fail(r, "Activityunit '"+ activityunit['name'] +"' creation error")
    activityunit['id'] = r.json()['id']
    print("Activityunit '"+ activityunit['name'] +"' created with ID:", activityunit['id'])



# Delete everything
##fail(None, None)
