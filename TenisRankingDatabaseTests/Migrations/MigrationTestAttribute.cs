﻿namespace ItBuildsXUnitTests.Migrations;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class MigrationTestAttribute : Attribute
{
    public int MigrationOrder { get; private set; }
    public MigrationTestAttribute(int migrationOrder) => MigrationOrder = migrationOrder;
}
