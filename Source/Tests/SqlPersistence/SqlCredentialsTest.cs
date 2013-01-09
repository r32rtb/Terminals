﻿using System;
using System.Data.Objects;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Terminals.Data;
using CredentialSet = Terminals.Data.DB.CredentialSet;

namespace Tests
{
    /// <summary>
    ///This is a test class for database implementation of StoredCredentials
    ///</summary>
    [TestClass]
    public class SqlCredentialsTest : SqlTestsLab
    {
        private const string TEST_PASSWORD = "aaa";

        private ICredentials PrimaryCredentials
        {
            get
            {
                return this.PrimaryPersistence.Credentials;
            }
        }

        private ObjectQuery<CredentialSet> CheckDatabaseCredentials
        {
            get
            {
                return this.CheckDatabase.CredentialBase
                    .OfType<CredentialSet>();
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            this.InitializeTestLab();
        }

        [TestCleanup]
        public void TestClose()
        {
            this.ClearTestLab();
        }

        private CredentialSet CreateTestCredentialSet()
        {
            CredentialSet credentials = this.PrimaryFactory.CreateCredentialSet() as CredentialSet;
            credentials.Name = "TestCredentialName";
            credentials.Domain = "TestDomain";
            credentials.UserName = "TestUserName";
            ((ICredentialSet)credentials).Password = TEST_PASSWORD;
            return credentials;
        }

        [TestMethod]
        public void AddCredentialsTest()
        {
            this.AddTestCredentialsToDatabase();

            var checkCredentialSet = this.SecondaryPersistence.Credentials.FirstOrDefault() as CredentialSet;

            Assert.IsNotNull(checkCredentialSet, "Credential didn't reach the database");
            Assert.AreEqual(TEST_PASSWORD, ((ICredentialSet)checkCredentialSet).Password, "Password doesn't match");
        }

        [TestMethod]
        public void RemoveCredentialsTest()
        {
            var testCredentials = this.AddTestCredentialsToDatabase();

            int credentialsCountBefore = this.CheckDatabaseCredentials.Count();
            PrimaryCredentials.Remove(testCredentials);
            int credentialsCountAfter = this.CheckDatabaseCredentials.Count();

            int baseAfter = this.CheckDatabase.ExecuteStoreQuery<int>("select Count(Id) from CredentialBase")
                .FirstOrDefault();

            Assert.AreEqual(1, credentialsCountBefore, "credential wasn't added to the database");
            Assert.AreEqual(0, credentialsCountAfter, "credential wasn't removed from the database");
            Assert.AreEqual(0, baseAfter, "credential base wasn't removed from the database");
        }

        private CredentialSet AddTestCredentialsToDatabase()
        {
            var testCredentials = this.CreateTestCredentialSet();
            PrimaryCredentials.Add(testCredentials);
            return testCredentials;
        }

        /// <summary>
        ///A test for UpdatePasswordsByNewKeyMaterial
        ///</summary>
        [TestMethod]
        public void UpdateCredentialsPasswordsByNewKeyMaterialTest()
        {
            this.AddTestCredentialsToDatabase();
            this.PrimaryPersistence.Security.UpdateMasterPassword(String.Empty);

            ICredentialSet checkCredentials = this.SecondaryPersistence.Credentials.FirstOrDefault();
            Assert.AreEqual(TEST_PASSWORD, checkCredentials.Password, "Password lost after update of key material");
        }
    }
}