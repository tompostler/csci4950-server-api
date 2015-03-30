# Tom Postler, 2015-03-26
# This set of modules parses all of the available UMN courses from the course
#   catalog and converts them to a Transact-SQL series of insert statements.

from urllib import request
from bs4 import BeautifulSoup
from sqldump import DumpToTransactSQL
from time import sleep
from random import randint
from sys import argv



def FromCampus(camp='UMNTC'):
    # Get the campus-specific page
    page = request.urlopen("http://onestop2.umn.edu/courses/designators.jsp?campus="+camp)

    # Get the soup
    soup = BeautifulSoup(page.read().decode())

    # Get all the options
    options = soup.find_all('option')
    # Get just the part we care about
    options = [option['value'] for option in options]

    return options



def FromDepartment(dept):
    # Get the department-specific page
    page = request.urlopen("http://onestop2.umn.edu/courses/courses.jsp?designator={}&submit=Show+the+courses&campus=UMNTC".format(dept))

    # Get the soup
    soup = BeautifulSoup(page.read().decode('iso-8859-1'))

    # Get all the spans
    spans = soup.find_all('span')
    # Get just the class num spans
    nums = [span for span in spans if span.get('class')==['bodysubtitlered']]
    # Get just the part we care about
    nums = [num.get_text().strip().replace('\t','').replace('\n',' ') for num in nums]

    # Get all the bs
    bs = soup.find_all('b')
    # Get just the first line of the bs
    names = [b.get_text().split('\n')[0] for b in bs]
    # Get just the part we care about
    names = [name[3:] for name in names if name[0:2]==' -']

    return (nums, names)



def GetAllCourses():
    # Get all the departments
    depts = FromCampus()

    allnums = []
    allnames = []

    for dept in depts:
        # Get the courses
        print("Retrieving courses from:", dept)
        nums, names = FromDepartment(dept)

        # If the lens match, add it to the list
        if (len(nums) == len(names)):
            allnums += nums
            allnames += names
            print(len(nums), "courses retrieved")
        else:
            print("MISMATCH for", dept, "nums={} names={}".format(len(nums),len(names)))

        # Sleep so we don't kill (or get blocked by) the server
        # Add any parameters to the script to skip this
        if len(argv) == 1:
            secs = randint(5,10)
            print("Sleeping for", secs, "secs")
            sleep(secs)

    return (allnums, allnames)



if __name__ == '__main__':
    # Get the courses
    nums, names = GetAllCourses()
    print('\n'+'-'*80+'\n')
    print(len(nums), "courses found")

    # Convert to Transact-SQL
    DumpToTransactSQL(nums, names)
