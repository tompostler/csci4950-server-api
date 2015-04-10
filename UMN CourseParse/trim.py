# Tom Postler, 2015-04-09
# Trim out any courses above a specific numeric level



def TrimCourses(nums, names, maxlevel):
    tnums = []
    tnames = []

    for i in range(len(nums)):
        try:
            # Split the name and grab the first digit
            level = int(nums[i].split()[1][0])

            # Append if good
            if level <= maxlevel:
                tnums.append(nums[i])
                tnames.append(names[i])
                
        except:
            print("PROBLEM:", nums[i])
            tnums.append(nums[i])
            tnames.append(names[i])

    return (tnums, tnames)
