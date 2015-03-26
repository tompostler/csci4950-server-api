# Tom Postler, 2015-03-26
# Dump two matching lists of course nums and names into the needed Transact-SQL
#   query and save it in a file



def DumpToTransactSQL(nums, names):
    sqltxt = """TRUNCATE TABLE [dbo].[courses];

GO

INSERT INTO [dbo].[courses] ([id], [name])
VALUES
""";

    sqlvals = []

    for i in range(len(nums)):
        # Check the lengths for the SQL Schema
        if len(nums[i]) > 12:
            print("ID   >12 ({}):".format(len(nums[i]), nums[i]))
        if len(names[i]) > 32:
            print("NAME >32 ({}):".format(len(names[i]), names[i]))

        sqlvals.append("(N'{}', N'{}')".format(nums[i], names[i]))

    # Join things together and finish the SQL
    sqltxt += ',\n'.join(sqlvals)
    sqltxt += ";\n\nGO\n"

    # Write the file out
    open("courses.sql", 'w')
    courses.write(sqltxt)
    courses.close()

    print("Wrote SQL to courses.sql")
