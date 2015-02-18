using System;

namespace Server_API.Models
{
    public class Activity
    {
        /// <summary>
        /// The ID.
        /// </summary>
        private int id;
        /// <summary>
        /// The corresponding user ID.
        /// </summary>
        private int user;
        /// <summary>
        /// The name of the activity, limited to 50 chars.
        /// </summary>
        private string name;
        /// <summary>
        /// The category.
        /// </summary>
        /// <remarks>
        /// In SQL, this is represented by a tinyint which corresponds to a sbyte.
        /// </remarks>
        private sbyte category;

        /// <summary>
        /// Getters and Setters for the ID.
        /// </summary>
        public int ID
        {
            // If id is null, return -1
            get
            {
                return id != null ? id : -1;
            }
            // If id is not >0, throw up
            set
            {
                if (value > 0)
                    id = value;
                else
                    throw new ArgumentException("ID must be >0", "value");
            }
        }

        /// <summary>
        /// Getters and Setters for the User.
        /// </summary>
        public int User
        {
            get
            {
                return user != null ? user : -1;
            }
            set
            {
                if (value > 0)
                    user = value;
                else
                    throw new ArgumentException("User must be >0", "value");
            }
        }

        /// <summary>
        /// Getters and Setters for the Name.
        /// </summary>
        public string Name
        {
            get
            {
                return name != null ? name : "";
            }
            set
            {
                if ((value.Length > 50) && (!String.IsNullOrEmpty(value)))
                    name = value;
                else
                    throw new ArgumentException("Name must be <50 and >0 in length", "value");
            }
        }

        /// <summary>
        /// Getters and Setters for the Category.
        /// </summary>
        public sbyte Category
        {
            get
            {
                return category != null ? category : (sbyte)(-1);
            }
            set
            {
                if ((value > 0) && (value <= SByte.MaxValue))
                    category = value;
                else
                    throw new ArgumentException("Category must be >0 and <SByte.MaxValue", "value");
            }
        }
    }
}