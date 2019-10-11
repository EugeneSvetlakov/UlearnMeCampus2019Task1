using System;
using System.Collections.Generic;
using System.Text;

namespace campus1
{
    class LinqSortingTest
    {
    }

    public class MyQueue:BaseEntity
    {
        public bool InPause { get; set; }

        public User User { get; set; }

        public Microwave Microwave { get; set; }
    }


    public class User : INamedEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Post Post { get; set; }
    }

    public class Post : NamedEntity, IOrderedEntity
    {
        public int Order { get; set; }
    }

    public class Microwave : NamedEntity
    {
        public Room Room { get; set; }
    }

    public class Room : NamedEntity
    {

    }

    public class BaseEntity : IBaseEntity
    {
        public int Id { get; set; }
    }

    public class NamedEntity : INamedEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public interface IBaseEntity
    {
        int Id { get; set; }
    }

    public interface INamedEntity : IBaseEntity
    {
        string Name { get; set; }
    }

    public interface IOrderedEntity
    {
        int Order { get; set; }
    }
}
