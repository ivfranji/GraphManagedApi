namespace Microsoft.Graph.Exchange
{
    /// <summary>
    /// Well known folder name.
    /// </summary>
    public enum WellKnownFolderName
    {
        /// <summary>
        /// The Calendar folder.
        /// </summary>
        Calendar,

        /// <summary>
        /// The Contacts folder.
        /// </summary>
        Contacts,

        /// <summary>
        /// The Deleted Items folder
        /// </summary>
        DeletedItems,

        /// <summary>
        /// The Drafts folder.
        /// </summary>
        Drafts,

        /// <summary>
        /// The Inbox folder.
        /// </summary>
        Inbox,

        /// <summary>
        /// The Journal folder.
        /// </summary>
        Journal,

        /// <summary>
        /// The Notes folder.
        /// </summary>
        Notes,

        /// <summary>
        /// The Outbox folder.
        /// </summary>
        Outbox,

        /// <summary>
        /// The Sent Items folder.
        /// </summary>
        SentItems,

        /// <summary>
        /// The Tasks folder.
        /// </summary>
        Tasks,

        /// <summary>
        /// The message folder root.
        /// </summary>
        MsgFolderRoot,

        /// <summary>
        /// The root of the Public Folders hierarchy.
        /// </summary>
        PublicFoldersRoot,

        /// <summary>
        /// The root of the mailbox.
        /// </summary>
        Root,

        /// <summary>
        /// The Junk E-mail folder.
        /// </summary>
        JunkEmail,

        /// <summary>
        /// The Search Folders folder, also known as the Finder folder.
        /// </summary>
        SearchFolders,

        /// <summary>
        /// The Voicemail folder.
        /// </summary>
        VoiceMail,

        /// <summary>
        /// The Dumpster 2.0 root folder.
        /// </summary>
        RecoverableItemsRoot,

        /// <summary>
        /// The Dumpster 2.0 soft deletions folder.
        /// </summary>
        RecoverableItemsDeletions,

        /// <summary>
        /// The Dumpster 2.0 versions folder.
        /// </summary>
        RecoverableItemsVersions,

        /// <summary>
        /// The Dumpster 2.0 hard deletions folder.
        /// </summary>
        RecoverableItemsPurges,

        /// <summary>
        /// The Dumpster 2.0 discovery hold folder
        /// </summary>
        RecoverableItemsDiscoveryHolds,

        /// <summary>
        /// The root of the archive mailbox.
        /// </summary>
        ArchiveRoot,

        /// <summary>
        /// The root of the archive mailbox.
        /// </summary>
        ArchiveInbox,

        /// <summary>
        /// The message folder root in the archive mailbox.
        /// </summary>
        ArchiveMsgFolderRoot,

        /// <summary>
        /// The Deleted Items folder in the archive mailbox
        /// </summary>
        ArchiveDeletedItems,

        /// <summary>
        /// The Dumpster 2.0 root folder in the archive mailbox.
        /// </summary>
        ArchiveRecoverableItemsRoot,

        /// <summary>
        /// The Dumpster 2.0 soft deletions folder in the archive mailbox.
        /// </summary>
        ArchiveRecoverableItemsDeletions,

        /// <summary>
        /// The Dumpster 2.0 versions folder in the archive mailbox.
        /// </summary>
        ArchiveRecoverableItemsVersions,

        /// <summary>
        /// The Dumpster 2.0 hard deletions folder in the archive mailbox.
        /// </summary>
        ArchiveRecoverableItemsPurges,

        /// <summary>
        /// The Dumpster 2.0 discovery hold folder in the archive mailbox.
        /// </summary>
        ArchiveRecoverableItemsDiscoveryHolds,

        /// <summary>
        /// The Sync Issues folder.
        /// </summary>
        SyncIssues,

        /// <summary>
        /// The Conflicts folder
        /// </summary>
        Conflicts,

        /// <summary>
        /// The Local failures folder
        /// </summary>
        LocalFailures,

        /// <summary>
        /// The Server failures folder
        /// </summary>
        ServerFailures,

        /// <summary>
        /// The Recipient Cache folder
        /// </summary>
        RecipientCache,

        /// <summary>
        /// The Quick Contacts folder
        /// </summary>
        QuickContacts,

        /// <summary>
        /// Conversation history folder
        /// </summary>
        ConversationHistory,

        /// <summary>
        /// AdminAuditLogs folder
        /// </summary>
        AdminAuditLogs,

        /// <summary>
        /// To Do search folder
        /// </summary>
        ToDoSearch,

        /// <summary>
        /// MyContacts folder
        /// </summary>
        MyContacts,

        /// <summary>
        /// Directory (GAL)
        /// It is not a mailbox folder. It only indicates any GAL operation.
        /// </summary>
        Directory,

        /// <summary>
        /// IMContactList folder
        /// </summary>
        IMContactList,

        /// <summary>
        /// PeopleConnect folder
        /// </summary>
        PeopleConnect,

        /// <summary>
        /// Favorites folder
        /// </summary>
        Favorites,
    }
}
