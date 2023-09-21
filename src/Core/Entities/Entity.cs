using System;
using Core.Interfaces;

namespace Core.Entities
{
	public abstract class Entity : IEntity
    {
        public int Id { get; set; }

    }
}

