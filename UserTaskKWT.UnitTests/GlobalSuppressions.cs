// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Tests can be named with underscores", Scope = "member", Target = "~M:UserTaskJWT.UnitTests.PasswordHasherUnitTests.Verify_ShouldReturnTrueForCorrectPassword")]
[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Tests can be named with underscores", Scope = "member", Target = "~M:UserTaskJWT.UnitTests.PasswordHasherUnitTests.Hash_ShouldGenerateDifferentHashesForSamePassword")]
[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Tests can be named with underscores", Scope = "member", Target = "~M:UserTaskJWT.UnitTests.PasswordHasherUnitTests.Hash_ShouldGenerateHashWithSalt")]
[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Tests can be named with underscores", Scope = "member", Target = "~M:UserTaskJWT.UnitTests.PasswordHasherUnitTests.Verify_ShouldReturnFalseForIncorrectPassword")]
[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Tests can be named with underscores", Scope = "member", Target = "~M:UserTaskJWT.UnitTests.PasswordHasherUnitTests.Verify_ShouldThrowArgumentNullExceptionForNullPassword")]
[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Tests can be named with underscores", Scope = "member", Target = "~M:UserTaskJWT.UnitTests.PasswordHasherUnitTests.Verify_ShouldThrowArgumentNullExceptionForNullPasswordHash")]
