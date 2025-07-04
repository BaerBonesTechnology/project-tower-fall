using UnityEngine;

namespace Baerhous.Games.Towerfall.Exceptions
{
    public class DungeonException : UnityException
    {
        public DungeonException(string message) : base(message)
        {
            
        }
    }

    public class InvalidDungeonException : UnityException
    {
        public InvalidDungeonException() : base("Invalid Dungeon")
        {
            
        }
    }
}