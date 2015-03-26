# Tom Postler, 2015-03-26
# Dump two matching lists of course nums and names into the needed Transact-SQL
#   query and save it in a file



def DumpToTransactSQL(nums, names):
    sqltxt = """TRUNCATE TABLE [dbo].[courses]""";

    sqlins = """INSERT INTO [dbo].[courses] ([id], [name])
VALUES
""";
    
    sqlgoc = """;

GO

""";

    sqlvals = []
    maxnum = 12
    maxnam = 100

    for i in range(len(nums)):
        # Max 1000 rows per insert statement
        if i%1000 == 0 and i != 0:
            # Join the values together
            sqltxt += ',\n'.join(sqlvals)
            # Add the ;GO to the text and the start of another insert
            sqltxt += sqlgoc + sqlins
            # Clear out the current list
            sqlvals = []
        
        # Check the lengths for the SQL Schema
        numlen = len(nums[i])
        namlen = len(names[i])
        if numlen > 12:
            if numlen > maxnum:
                maxnum = numlen
            print("ID   >12 ({}):".format(numlen, nums[i]))
        if namlen > 100:
            if namlen > maxnam:
                maxnam = namlen
            print("NAME >100 ({0:3}) ({1:12}):".format(namlen, nums[i]), names[i])

        sqlvals.append("(N'{}', N'{}')".format(nums[i], names[i].replace("'","''")))

    # Cleanup
    sqltxt += ',\n'.join(sqlvals)
    sqltxt += sqlgoc

    # Info
    print("Maximum num length found:", maxnum)
    print("Maximum name length found:", maxnam)

    # Write the file out
    courses = open("courses.sql", 'w')
    courses.write(sqltxt)
    courses.close()

    print("Wrote SQL to courses.sql")
