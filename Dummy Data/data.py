# Tom Postler, 2015-04-15
# The data (modelled off my schedule from 2 years ago)

# inserts user_id after getting it where necessary

##-+-----+-----+-----+-----+-----+-----++-----+-----+-----+-----+-----+-----+-##
user = {
    'fname': "Tom",
    'lname': "Postler",
    'email': "tcp@umn.edu",
    'password': "#yolo"
}


##-+-----+-----+-----+-----+-----+-----++-----+-----+-----+-----+-----+-----+-##
locations = [
    {   #0
        'name': "KHKH 3-125",
        'content': "44.974533, -93.232184"
    },
    {
        'name': "Phys 166",
        'content': "44.975106, -93.234373"
    },
    {   #2
        'name': "MechE 18",
        'content': "44.975467, -93.233622"
    },
    {
        'name': "HMH 1-108",
        'content': "44.969756, -93.244513"
    },
    {   #4
        'name': "KHKH 3-210",
        'content': "44.974313, -93.232548"
    },
    {
        'name': "KHKH 2-120",
        'content': "44.974533, -93.232184"
    },
    {   #6
        'name': "KHKH 2-260",
        'content': "44.974313, -93.232548"
    },
    {
        'name': "CarlSMgmt L-110",
        'content': "44.970387, -93.244860"
    },
    {   #8
        'name': "KHKH 5-212",
        'content': "44.974313, -93.232548"
    },
    {
        'name': "Home",
        'content': "44.980616, -93.240418"
    },
    {   #10
        'name': "Dentist Office",
        'content': "44.972301, -93.231756"
    }
]


##-+-----+-----+-----+-----+-----+-----++-----+-----+-----+-----+-----+-----+-##
activities = [
    {   #0
        'course_id': "CSCI 2021",
        'name': "Lab 2 - bomblab",
        'description': "Diffusing the binary bomb with gdb",
        'ddate': "2013-02-26T05:55:00Z",
        'eduration': 14,
        'pduration': 9,
##        'importance': ,
##        'tag_ids': []
    },
    {
        'course_id': "ACCT 2050",
        'name': "Quiz 2 (Ch 4-5)",
##        'description': "",
        'ddate': "2013-02-25T20:00:00Z",
        'eduration': 2,
        'pduration': 2,
##        'importance': "",
##        'tag_ids': []
    },
    {   #2
        'course_id': "STAT 3021",
        'name': "HW 3",
        'description': "Basic statistics",
        'ddate': "2013-02-27T21:35:00Z",
        'eduration': 3,
        'pduration': 1.5,
##        'importance': "",
##        'tag_ids': []
    },
    {
##        'course_id': "",
        'name': "Dentist appointment",
        'description': "Go get a cavity filled...",
##        'ddate': "",
##        'eduration': ,
##        'pduration': ,
##        'importance': "",
##        'tag_ids': []
    },
    {   #4
##        'course_id': "",
        'name': "CSCI 1113 TAing",
##        'description': "",
##        'ddate': "",
##        'eduration': ,
##        'pduration': ,
##        'importance': "",
##        'tag_ids': []
    },
    {
        'course_id': "CSCI 4041",
        'name': "4041 Exam 1",
        'description': "studying time for exam 1",
        'ddate': "2013-02-28T19:00:00Z",
        'eduration': 3,
        'pduration': 4,
##        'importance': "",
##        'tag_ids': []
    },
    {   #6
        'course_id': "CSCI 2021",
        'name': "2021 Exam 1",
        'description': "studying time for exam 1",
        'ddate': "2013-03-01T15:15:00Z",
        'eduration': 8,
        'pduration': 6,
##        'importance': "",
##        'tag_ids': []
    },
    {
        'course_id': "ACCT 2050",
        'name': "ACCT Exam 1",
        'description': "studying time for exam 1",
        'ddate': "2013-02-27T18:20:00Z",
        'eduration': 3,
        'pduration': 2,
##        'importance': "",
##        'tag_ids': []
    },
##    {
##        'course_id': "",
##        'name': "",
##        'description': "",
##        'ddate': "",
##        'eduration': ,
##        'pduration': ,
##        'importance': "",
##        'tag_ids': []
##    }
]


##-+-----+-----+-----+-----+-----+-----++-----+-----+-----+-----+-----+-----+-##
# activity_id and location_id are initially the list index
# update with UpdateActivityUnits()
activityunits = [
    {
        'activity_id': 0,
        'location_id': 9,
        'name': "Bomblab phase 1-5",
        'description': "was able to finish up through phase 5",
        'stime': "2013-02-24T20:15:13Z",
        'etime': "2013-02-25T05:27:18Z",
##        'tag_ids': []
    },
    {
        'activity_id': 1,
        'location_id': 7,
        'name': "ACCT 2050 Quiz 2",
##        'description': "",
        'stime': "2013-02-25T18:00:00Z",
        'etime': "2013-02-25T20:00:00Z",
##        'tag_ids': []
    },
    {
        'activity_id': 2,
        'location_id': 9,
        'name': "HW3 work",
        'description': "First third done",
        'stime': "2013-02-25T20:46:57Z",
        'etime': "2013-02-25T21:18:45Z",
##        'tag_ids': []
    },
    {
        'activity_id': 2,
        'location_id': 9,
        'name': "HW3 work",
        'description': "Second third done",
        'stime': "2013-02-25T23:57:14Z",
        'etime': "2013-02-26T01:02:03Z",
##        'tag_ids': []
    },
    {
        'activity_id': 3,
        'location_id': 10,
        'name': "Dentist",
##        'description': "",
        'stime': "2013-02-26T14:00:00Z",
        'etime': "2013-02-26T15:00:00Z",
##        'tag_ids': []
    },
    {
        'activity_id': 5,
        'location_id': 9,
        'name': "studying for exam 1",
##        'description': "",
        'stime': "2013-02-27T20:03:14Z",
        'etime': "2013-02-28T00:05:45Z",
##        'tag_ids': []
    },
    {
        'activity_id': 6,
        'location_id': 9,
        'name': "studying time for exam 1",
##        'description': "",
        'stime': "2013-03-01T12:48:16Z",
        'etime': "2013-03-01T16:52:34Z",
##        'tag_ids': []
    },
    {
        'activity_id': 6,
        'location_id': 9,
        'name': "studying time for exam 1",
##        'description': "",
        'stime': "2013-03-01T02:15:33Z",
        'etime': "2013-03-01T04:13:22Z",
##        'tag_ids': []
    },
    {
        'activity_id': 7,
        'location_id': 9,
        'name': "studying time for exam 1",
##        'description': "",
        'stime': "2013-02-27T15:03:05Z",
        'etime': "2013-02-27T17:16:15Z",
##        'tag_ids': []
    },
    {
        'activity_id': 4,
        'location_id': 8,
        'name': "TA weekly meeting",
        'description': "short weekly update",
        'stime': "2013-02-25T15:30:00Z",
        'etime': "2013-02-25T16:00:00Z",
##        'tag_ids': []
    },
    {
        'activity_id': 4,
        'location_id': 5,
        'name': "TA Lab",
        'description': "section 003",
        'stime': "2013-02-25T23:45:00Z",
        'etime': "2013-02-26T03:45:00Z",
##        'tag_ids': []
    },
    {
        'activity_id': 4,
        'location_id': 5,
        'name': "TA Lab",
        'description': "section 013",
        'stime': "2013-02-27T23:45:00Z",
        'etime': "2013-02-28T03:45:00Z",
##        'tag_ids': []
    },
    {
        'activity_id': 4,
        'location_id': 5,
        'name': "TA Office hours",
##        'description': "",
        'stime': "2013-02-28T15:00:00Z",
        'etime': "2013-02-28T17:00:00Z",
##        'tag_ids': []
    },
##    {
##        'activity_id': ,
##        'location_id': ,
##        'name': "",
##        'description': "",
##        'stime': "",
##        'etime': "",
##        'tag_ids': []
##    },
]

def UpdateActivityUnits():
    for activityunit in activityunits:
        activityunit['activity_id'] = activities[activityunit['activity_id']]['id']
        activityunit['location_id'] = locations[activityunit['location_id']]['id']
