﻿//
// MailFolder.cs
//
// Author: Jeffrey Stedfast <jeff@xamarin.com>
//
// Copyright (c) 2014 Xamarin Inc. (www.xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;

using MimeKit;

using MailKit.Search;

namespace MailKit {
	/// <summary>
	/// An abstract mail folder implementation.
	/// </summary>
	/// <remarks>
	/// An abstract mail folder implementation.
	/// </remarks>
	public abstract class MailFolder : IMailFolder
	{
		/// <summary>
		/// Get the parent folder.
		/// </summary>
		/// <remarks>
		/// Root-level folders do not have a parent folder.
		/// </remarks>
		/// <value>The parent folder.</value>
		public IMailFolder ParentFolder {
			get; protected set;
		}

		/// <summary>
		/// Get the folder attributes.
		/// </summary>
		/// <remarks>
		/// Gets the folder attributes.
		/// </remarks>
		/// <value>The folder attributes.</value>
		public FolderAttributes Attributes {
			get; protected set;
		}

		/// <summary>
		/// Get the permanent flags.
		/// </summary>
		/// <remarks>
		/// The permanent flags are the message flags that will persist between sessions.
		/// </remarks>
		/// <value>The permanent flags.</value>
		public MessageFlags PermanentFlags {
			get; protected set;
		}

		/// <summary>
		/// Get the accepted flags.
		/// </summary>
		/// <remarks>
		/// The accepted flags are the message flags that will be accepted and persist
		/// for the current session. For the set of flags that will persist between
		/// sessions, see the <see cref="PermanentFlags"/> property.
		/// </remarks>
		/// <value>The accepted flags.</value>
		public MessageFlags AcceptedFlags {
			get; protected set;
		}

		/// <summary>
		/// Get the directory separator.
		/// </summary>
		/// <remarks>
		/// Gets the directory separator.
		/// </remarks>
		/// <value>The directory separator.</value>
		public char DirectorySeparator {
			get; protected set;
		}

		/// <summary>
		/// Get the read/write access of the folder.
		/// </summary>
		/// <remarks>
		/// Gets the read/write access of the folder.
		/// </remarks>
		/// <value>The read/write access.</value>
		public FolderAccess Access {
			get; protected set;
		}

		/// <summary>
		/// Get whether or not the folder is a namespace folder.
		/// </summary>
		/// <remarks>
		/// Gets whether or not the folder is a namespace folder.
		/// </remarks>
		/// <value><c>true</c> if the folder is a namespace folder; otherwise, <c>false</c>.</value>
		public bool IsNamespace {
			get; protected set;
		}

		/// <summary>
		/// Get the full name of the folder.
		/// </summary>
		/// <remarks>
		/// This is the equivalent of the full path of a file on a file system.
		/// </remarks>
		/// <value>The full name of the folder.</value>
		public string FullName {
			get; protected set;
		}

		/// <summary>
		/// Get the name of the folder.
		/// </summary>
		/// <remarks>
		/// This is the equivalent of the file name of a file on the file system.
		/// </remarks>
		/// <value>The name of the folder.</value>
		public string Name {
			get; protected set;
		}

		/// <summary>
		/// Get a value indicating whether the folder is subscribed.
		/// </summary>
		/// <remarks>
		/// Gets a value indicating whether the folder is subscribed.
		/// </remarks>
		/// <value><c>true</c> if the folder is subscribed; otherwise, <c>false</c>.</value>
		public bool IsSubscribed {
			get; protected set;
		}

		/// <summary>
		/// Get a value indicating whether the folder is currently open.
		/// </summary>
		/// <remarks>
		/// Gets a value indicating whether the folder is currently open.
		/// </remarks>
		/// <value><c>true</c> if the folder is currently open; otherwise, <c>false</c>.</value>
		public abstract bool IsOpen {
			get;
		}

		/// <summary>
		/// Get a value indicating whether the folder exists.
		/// </summary>
		/// <remarks>
		/// Gets a value indicating whether the folder exists.
		/// </remarks>
		/// <value><c>true</c> if the folder exists; otherwise, <c>false</c>.</value>
		public bool Exists {
			get; protected set;
		}

		/// <summary>
		/// Get whether or not the folder supports mod-sequences.
		/// </summary>
		/// <remarks>
		/// <para>If mod-sequences are not supported by the folder, then all of the APIs that take a modseq
		/// argument will throw <see cref="System.NotSupportedException"/> and should not be used.</para>
		/// </remarks>
		/// <value><c>true</c> if supports mod-sequences; otherwise, <c>false</c>.</value>
		public bool SupportsModSeq {
			get; protected set;
		}

		/// <summary>
		/// Get the highest mod-sequence value of all messages in the mailbox.
		/// </summary>
		/// <remarks>
		/// Gets the highest mod-sequence value of all messages in the mailbox.
		/// </remarks>
		/// <value>The highest mod-sequence value.</value>
		public ulong HighestModSeq {
			get; protected set;
		}

		/// <summary>
		/// Get the UID validity.
		/// </summary>
		/// <remarks>
		/// <para>UIDs are only valid so long as the UID validity value remains unchanged. If and when
		/// the folder's <see cref="UidValidity"/> is changed, a client MUST discard its cache of UIDs
		/// along with any summary information that it may have and re-query the folder.</para>
		/// <para>This value will only be set after the folder has been opened.</para>
		/// </remarks>
		/// <value>The UID validity.</value>
		public UniqueId? UidValidity {
			get; protected set;
		}

		/// <summary>
		/// Get the UID that the folder will assign to the next message that is added.
		/// </summary>
		/// <remarks>
		/// This value will only be set after the folder has been opened.
		/// </remarks>
		/// <value>The next UID.</value>
		public UniqueId? UidNext {
			get; protected set;
		}

		/// <summary>
		/// Get the index of the first unread message in the folder.
		/// </summary>
		/// <remarks>
		/// This value will only be set after the folder has been opened.
		/// </remarks>
		/// <value>The index of the first unread message.</value>
		public int FirstUnread {
			get; protected set;
		}

		/// <summary>
		/// Get the number of unread messages in the folder.
		/// </summary>
		/// <remarks>
		/// This value will only be set after calling <see cref="Status(StatusItems, System.Threading.CancellationToken)"/>
		/// with <see cref="StatusItems.Unread"/>.
		/// </remarks>
		/// <value>The number of unread messages.</value>
		public int Unread {
			get; protected set;
		}

		/// <summary>
		/// Get the number of recently added messages.
		/// </summary>
		/// <remarks>
		/// Gets the number of recently added messages.
		/// </remarks>
		/// <value>The number of recently added messages.</value>
		public int Recent {
			get; protected set;
		}

		/// <summary>
		/// Get the total number of messages in the folder.
		/// </summary>
		/// <remarks>
		/// Gets the total number of messages in the folder.
		/// </remarks>
		/// <value>The total number of messages.</value>
		public int Count {
			get; protected set;
		}

		/// <summary>
		/// Opens the folder using the requested folder access.
		/// </summary>
		/// <remarks>
		/// <para>This variant of the <see cref="Open(FolderAccess,System.Threading.CancellationToken)"/>
		/// method is meant for quick resynchronization of the folder. Before calling this method,
		/// the <see cref="MailStore.EnableQuickResync(CancellationToken)"/> method MUST be called.</para>
		/// <para>You should also make sure to add listeners to the <see cref="MessagesVanished"/> and
		/// <see cref="MessageFlagsChanged"/> events to get notifications of changes since
		/// the last time the folder was opened.</para>
		/// </remarks>
		/// <returns>The <see cref="FolderAccess"/> state of the folder.</returns>
		/// <param name="access">The requested folder access.</param>
		/// <param name="uidValidity">The last known <see cref="UidValidity"/> value.</param>
		/// <param name="highestModSeq">The last known <see cref="HighestModSeq"/> value.</param>
		/// <param name="uids">The last known list of unique message identifiers.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="access"/> is not a valid value.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is either not connected or not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The quick resynchronization feature has not been enabled.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// The mail store does not support the quick resynchronization feature.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract FolderAccess Open (FolderAccess access, UniqueId uidValidity, ulong highestModSeq, UniqueId[] uids, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously opens the folder using the requested folder access.
		/// </summary>
		/// <remarks>
		/// <para>This variant of the <see cref="OpenAsync(FolderAccess,System.Threading.CancellationToken)"/>
		/// method is meant for quick resynchronization of the folder. Before calling this method,
		/// the <see cref="MailStore.EnableQuickResync(CancellationToken)"/> method MUST be called.</para>
		/// <para>You should also make sure to add listeners to the <see cref="MessagesVanished"/> and
		/// <see cref="MessageFlagsChanged"/> events to get notifications of changes since
		/// the last time the folder was opened.</para>
		/// </remarks>
		/// <returns>The <see cref="FolderAccess"/> state of the folder.</returns>
		/// <param name="access">The requested folder access.</param>
		/// <param name="uidValidity">The last known <see cref="UidValidity"/> value.</param>
		/// <param name="highestModSeq">The last known <see cref="HighestModSeq"/> value.</param>
		/// <param name="uids">The last known list of unique message identifiers.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="access"/> is not a valid value.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is either not connected or not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The quick resynchronization feature has not been enabled.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// The mail store does not support the quick resynchronization feature.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<FolderAccess> OpenAsync (FolderAccess access, UniqueId uidValidity, ulong highestModSeq, UniqueId[] uids, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (access != FolderAccess.ReadOnly && access != FolderAccess.ReadWrite)
				throw new ArgumentOutOfRangeException ("access");

			if (uids == null)
				throw new ArgumentNullException ("uids");

			if (uids.Length == 0)
				throw new ArgumentException ("No uids were specified.", "uids");

			return Task.Factory.StartNew (() => {
				return Open (access, uidValidity, highestModSeq, uids, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Open the folder using the requested folder access.
		/// </summary>
		/// <remarks>
		/// Opens the folder using the requested folder access.
		/// </remarks>
		/// <returns>The <see cref="FolderAccess"/> state of the folder.</returns>
		/// <param name="access">The requested folder access.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="access"/> is not a valid value.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="IMailStore"/> is either not connected or not authenticated.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract FolderAccess Open (FolderAccess access, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously open the folder using the requested folder access.
		/// </summary>
		/// <remarks>
		/// Asynchronously opens the folder using the requested folder access.
		/// </remarks>
		/// <returns>The <see cref="FolderAccess"/> state of the folder.</returns>
		/// <param name="access">The requested folder access.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="access"/> is not a valid value.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="IMailStore"/> is either not connected or not authenticated.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<FolderAccess> OpenAsync (FolderAccess access, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (access != FolderAccess.ReadOnly && access != FolderAccess.ReadWrite)
				throw new ArgumentOutOfRangeException ("access");

			return Task.Factory.StartNew (() => {
				return Open (access, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Close the folder, optionally expunging the messages marked for deletion.
		/// </summary>
		/// <remarks>
		/// Closes the folder, optionally expunging the messages marked for deletion.
		/// </remarks>
		/// <param name="expunge">If set to <c>true</c>, expunge.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract void Close (bool expunge = false, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously close the folder, optionally expunging the messages marked for deletion.
		/// </summary>
		/// <remarks>
		/// Asynchronously closes the folder, optionally expunging the messages marked for deletion.
		/// </remarks>
		/// <param name="expunge">If set to <c>true</c>, expunge.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task CloseAsync (bool expunge = false, CancellationToken cancellationToken = default (CancellationToken))
		{
			return Task.Factory.StartNew (() => {
				Close (expunge, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Create a new subfolder with the given name.
		/// </summary>
		/// <remarks>
		/// Creates a new subfolder with the given name.
		/// </remarks>
		/// <returns>The created folder.</returns>
		/// <param name="name">The name of the folder to create.</param>
		/// <param name="isMessageFolder"><c>true</c> if the folder will be used to contain messages; otherwise <c>false</c>.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="name"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="name"/> is empty.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is either not connected or not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="DirectorySeparator"/> is nil, and thus child folders cannot be created.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract IMailFolder Create (string name, bool isMessageFolder, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously create a new subfolder with the given name.
		/// </summary>
		/// <remarks>
		/// Asynchronously creates a new subfolder with the given name.
		/// </remarks>
		/// <returns>The created folder.</returns>
		/// <param name="name">The name of the folder to create.</param>
		/// <param name="isMessageFolder"><c>true</c> if the folder will be used to contain messages; otherwise <c>false</c>.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="name"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="name"/> is empty.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is either not connected or not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="DirectorySeparator"/> is nil, and thus child folders cannot be created.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<IMailFolder> CreateAsync (string name, bool isMessageFolder, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (name == null)
				throw new ArgumentNullException ("name");

			if (name.Length == 0 || name.IndexOf (DirectorySeparator) != -1)
				throw new ArgumentException ("The name is not a legal folder name.", "name");

			return Task.Factory.StartNew (() => {
				return Create (name, isMessageFolder, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Rename the folder.
		/// </summary>
		/// <remarks>
		/// Renames the folder.
		/// </remarks>
		/// <param name="parent">The new parent folder.</param>
		/// <param name="name">The new name of the folder.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="parent"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="name"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="parent"/> does not belong to the <see cref="IMailStore"/>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="name"/> is not a legal folder name.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is either not connected or not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder cannot be renamed (it is either a namespace or the Inbox).</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract void Rename (IMailFolder parent, string name, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously rename the folder.
		/// </summary>
		/// <remarks>
		/// Asynchronously renames the folder.
		/// </remarks>
		/// <param name="parent">The new parent folder.</param>
		/// <param name="name">The new name of the folder.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="parent"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="name"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="parent"/> does not belong to the <see cref="IMailStore"/>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="name"/> is not a legal folder name.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is either not connected or not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder cannot be renamed (it is either a namespace or the Inbox).</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task RenameAsync (IMailFolder parent, string name, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (parent == null)
				throw new ArgumentNullException ("parent");

			if (name == null)
				throw new ArgumentNullException ("name");

			if (name.Length == 0 || name.IndexOf (parent.DirectorySeparator) != -1)
				throw new ArgumentException ("The name is not a legal folder name.", "name");

			if (IsNamespace)
				throw new InvalidOperationException ("Cannot rename this folder.");

			return Task.Factory.StartNew (() => {
				Rename (parent, name, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Delete the folder.
		/// </summary>
		/// <remarks>
		/// Deletes the folder.
		/// </remarks>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is either not connected or not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder cannot be deleted (it is either a namespace or the Inbox).</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract void Delete (CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously delete the folder.
		/// </summary>
		/// <remarks>
		/// Asynchronously deletes the folder.
		/// </remarks>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is either not connected or not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder cannot be deleted (it is either a namespace or the Inbox).</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task DeleteAsync (CancellationToken cancellationToken = default (CancellationToken))
		{
			if (IsNamespace)
				throw new InvalidOperationException ("Cannot delete this folder.");

			return Task.Factory.StartNew (() => {
				Delete (cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Subscribe to the folder.
		/// </summary>
		/// <remarks>
		/// Subscribes to the folder.
		/// </remarks>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="IMailStore"/> is either not connected or not authenticated.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract void Subscribe (CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously subscribe to the folder.
		/// </summary>
		/// <remarks>
		/// Asynchronously subscribes to the folder.
		/// </remarks>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="IMailStore"/> is either not connected or not authenticated.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task SubscribeAsync (CancellationToken cancellationToken = default (CancellationToken))
		{
			return Task.Factory.StartNew (() => {
				Subscribe (cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Unsubscribe from the folder.
		/// </summary>
		/// <remarks>
		/// Unsubscribes from the folder.
		/// </remarks>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="IMailStore"/> is either not connected or not authenticated.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract void Unsubscribe (CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously unsubscribe from the folder.
		/// </summary>
		/// <remarks>
		/// Asynchronously unsubscribes from the folder.
		/// </remarks>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="IMailStore"/> is either not connected or not authenticated.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task UnsubscribeAsync (CancellationToken cancellationToken = default (CancellationToken))
		{
			return Task.Factory.StartNew (() => {
				Unsubscribe (cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Get the subfolders.
		/// </summary>
		/// <remarks>
		/// Gets the subfolders.
		/// </remarks>
		/// <returns>The subfolders.</returns>
		/// <param name="subscribedOnly">If set to <c>true</c>, only subscribed folders will be listed.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="IMailStore"/> is either not connected or not authenticated.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract IEnumerable<IMailFolder> GetSubfolders (bool subscribedOnly = false, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously get the subfolders.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets the subfolders.
		/// </remarks>
		/// <returns>The subfolders.</returns>
		/// <param name="subscribedOnly">If set to <c>true</c>, only subscribed folders will be listed.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="IMailStore"/> is either not connected or not authenticated.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<IEnumerable<IMailFolder>> GetSubfoldersAsync (bool subscribedOnly = false, CancellationToken cancellationToken = default (CancellationToken))
		{
			return Task.Factory.StartNew (() => {
				return GetSubfolders (subscribedOnly, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Get the specified subfolder.
		/// </summary>
		/// <remarks>
		/// Gets the specified subfolder.
		/// </remarks>
		/// <returns>The subfolder.</returns>
		/// <param name="name">The name of the subfolder.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="name"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="name"/> is either an empty string or contains the <see cref="DirectorySeparator"/>.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="IMailStore"/> is either not connected or not authenticated.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="FolderNotFoundException">
		/// The requested folder could not be found.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract IMailFolder GetSubfolder (string name, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously get the specified subfolder.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets the specified subfolder.
		/// </remarks>
		/// <returns>The subfolder.</returns>
		/// <param name="name">The name of the subfolder.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="name"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="name"/> is either an empty string or contains the <see cref="DirectorySeparator"/>.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="IMailStore"/> is either not connected or not authenticated.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="FolderNotFoundException">
		/// The requested folder could not be found.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<IMailFolder> GetSubfolderAsync (string name, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (name == null)
				throw new ArgumentNullException ("name");

			if (name.Length == 0 || name.IndexOf (DirectorySeparator) != -1)
				throw new ArgumentException ("The name of the subfolder is invalid.", "name");

			return Task.Factory.StartNew (() => {
				return GetSubfolder (name, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Force the server to flush its state for the folder.
		/// </summary>
		/// <remarks>
		/// Forces the server to flush its state for the folder.
		/// </remarks>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract void Check (CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously force the server to flush its state for the folder.
		/// </summary>
		/// <remarks>
		/// Asynchronously forces the server to flush its state for the folder.
		/// </remarks>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task CheckAsync (CancellationToken cancellationToken = default (CancellationToken))
		{
			return Task.Factory.StartNew (() => {
				Check (cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Update the values of the specified items.
		/// </summary>
		/// <remarks>
		/// <para>Updates the values of the specified items.</para>
		/// <para>The <see cref="Status(StatusItems, System.Threading.CancellationToken)"/> method
		/// MUST NOT be used on a folder that is already in the opened state. Instead, other ways
		/// of getting the desired information should be used.</para>
		/// <para>For example, a common use for the <see cref="Status(StatusItems,System.Threading.CancellationToken)"/>
		/// method is to get the number of unread messages in the folder. When the folder is open, however, it is
		/// possible to use the <see cref="MailFolder.Search(MailKit.Search.SearchQuery, System.Threading.CancellationToken)"/>
		/// method to query for the list of unread messages.</para>
		/// </remarks>
		/// <param name="items">The items to update.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// The mail store does not support the STATUS command.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract void Status (StatusItems items, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously update the values of the specified items.
		/// </summary>
		/// <remarks>
		/// <para>Updates the values of the specified items.</para>
		/// <para>The <see cref="Status(StatusItems, System.Threading.CancellationToken)"/> method
		/// MUST NOT be used on a folder that is already in the opened state. Instead, other ways
		/// of getting the desired information should be used.</para>
		/// <para>For example, a common use for the <see cref="Status(StatusItems,System.Threading.CancellationToken)"/>
		/// method is to get the number of unread messages in the folder. When the folder is open, however, it is
		/// possible to use the <see cref="MailFolder.Search(MailKit.Search.SearchQuery, System.Threading.CancellationToken)"/>
		/// method to query for the list of unread messages.</para>
		/// </remarks>
		/// <param name="items">The items to update.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// The mail store does not support the STATUS command.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task StatusAsync (StatusItems items, CancellationToken cancellationToken = default (CancellationToken))
		{
			return Task.Factory.StartNew (() => {
				Status (items, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Expunge the folder, permanently removing all messages marked for deletion.
		/// </summary>
		/// <remarks>
		/// <para>Normally, an <see cref="MessageExpunged"/> event will be emitted for each
		/// message that is expunged. However, if the mail store supports the quick
		/// resynchronization feature and it has been enabled via the
		/// <see cref="MailStore.EnableQuickResync(CancellationToken)"/> method, then
		/// the <see cref="MessagesVanished"/> event will be emitted rather than the
		/// <see cref="MessageExpunged"/> event.</para>
		/// </remarks>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open in read-write mode.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract void Expunge (CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously expunge the folder, permanently removing all messages marked for deletion.
		/// </summary>
		/// <remarks>
		/// <para>Normally, an <see cref="MessageExpunged"/> event will be emitted for each
		/// message that is expunged. However, if the mail store supports the quick
		/// resynchronization feature and it has been enabled via the
		/// <see cref="MailStore.EnableQuickResync(CancellationToken)"/> method, then
		/// the <see cref="MessagesVanished"/> event will be emitted rather than the
		/// <see cref="MessageExpunged"/> event.</para>
		/// </remarks>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open in read-write mode.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task ExpungeAsync (CancellationToken cancellationToken = default (CancellationToken))
		{
			return Task.Factory.StartNew (() => {
				Expunge (cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Expunge the specified uids, permanently removing them from the folder.
		/// </summary>
		/// <remarks>
		/// <para>Normally, an <see cref="MessageExpunged"/> event will be emitted for each
		/// message that is expunged. However, if the mail store supports the quick
		/// resynchronization feature and it has been enabled via the
		/// <see cref="MailStore.EnableQuickResync(CancellationToken)"/> method, then
		/// the <see cref="MessagesVanished"/> event will be emitted rather than the
		/// <see cref="MessageExpunged"/> event.</para>
		/// </remarks>
		/// <param name="uids">The message uids.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="uids"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="uids"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="uids"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open in read-write mode.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract void Expunge (UniqueId[] uids, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously expunge the specified uids, permanently removing them from the folder.
		/// </summary>
		/// <remarks>
		/// <para>Normally, an <see cref="MessageExpunged"/> event will be emitted for each
		/// message that is expunged. However, if the mail store supports the quick
		/// resynchronization feature and it has been enabled via the
		/// <see cref="MailStore.EnableQuickResync(CancellationToken)"/> method, then
		/// the <see cref="MessagesVanished"/> event will be emitted rather than the
		/// <see cref="MessageExpunged"/> event.</para>
		/// </remarks>
		/// <param name="uids">The message uids.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="uids"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="uids"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="uids"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open in read-write mode.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task ExpungeAsync (UniqueId[] uids, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (uids == null)
				throw new ArgumentNullException ("uids");

			if (uids.Length == 0)
				throw new ArgumentException ("No uids were specified.", "uids");

			return Task.Factory.StartNew (() => {
				Expunge (uids, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Append the specified message to the folder.
		/// </summary>
		/// <remarks>
		/// Appends the specified message to the folder.
		/// </remarks>
		/// <returns>The UID of the appended message, if available; otherwise, <c>null</c>.</returns>
		/// <param name="message">The message.</param>
		/// <param name="flags">The message flags.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="message"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="IMailStore"/> is either not connected or not authenticated.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract UniqueId? Append (MimeMessage message, MessageFlags flags, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously append the specified message to the folder.
		/// </summary>
		/// <remarks>
		/// Asynchronously appends the specified message to the folder.
		/// </remarks>
		/// <returns>The UID of the appended message, if available; otherwise, <c>null</c>.</returns>
		/// <param name="message">The message.</param>
		/// <param name="flags">The message flags.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="message"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="IMailStore"/> is either not connected or not authenticated.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<UniqueId?> AppendAsync (MimeMessage message, MessageFlags flags, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (message == null)
				throw new ArgumentNullException ("message");

			return Task.Factory.StartNew (() => {
				return Append (message, flags, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Append the specified message to the folder.
		/// </summary>
		/// <remarks>
		/// Appends the specified message to the folder.
		/// </remarks>
		/// <returns>The UID of the appended message, if available; otherwise, <c>null</c>.</returns>
		/// <param name="message">The message.</param>
		/// <param name="flags">The message flags.</param>
		/// <param name="date">The received date of the message.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="message"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="IMailStore"/> is either not connected or not authenticated.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract UniqueId? Append (MimeMessage message, MessageFlags flags, DateTimeOffset date, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously append the specified message to the folder.
		/// </summary>
		/// <remarks>
		/// Asynchronously appends the specified message to the folder.
		/// </remarks>
		/// <returns>The UID of the appended message, if available; otherwise, <c>null</c>.</returns>
		/// <param name="message">The message.</param>
		/// <param name="flags">The message flags.</param>
		/// <param name="date">The received date of the message.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="message"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="IMailStore"/> is either not connected or not authenticated.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<UniqueId?> AppendAsync (MimeMessage message, MessageFlags flags, DateTimeOffset date, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (message == null)
				throw new ArgumentNullException ("message");

			return Task.Factory.StartNew (() => {
				return Append (message, flags, date, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Append the specified messages to the folder.
		/// </summary>
		/// <remarks>
		/// Appends the specified messages to the folder.
		/// </remarks>
		/// <returns>The UIDs of the appended messages, if available; otherwise, <c>null</c>.</returns>
		/// <param name="messages">The array of messages to append to the folder.</param>
		/// <param name="flags">The message flags to use for each message.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="messages"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="flags"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para>One or more of the <paramref name="messages"/> is null.</para>
		/// <para>-or-</para>
		/// <para>The number of messages does not match the number of flags.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="IMailStore"/> is either not connected or not authenticated.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract UniqueId[] Append (MimeMessage[] messages, MessageFlags[] flags, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously append the specified messages to the folder.
		/// </summary>
		/// <remarks>
		/// Asynchronously appends the specified messages to the folder.
		/// </remarks>
		/// <returns>The UIDs of the appended messages, if available; otherwise, <c>null</c>.</returns>
		/// <param name="messages">The array of messages to append to the folder.</param>
		/// <param name="flags">The message flags to use for each message.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="messages"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="flags"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para>One or more of the <paramref name="messages"/> is null.</para>
		/// <para>-or-</para>
		/// <para>The number of messages does not match the number of flags.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="IMailStore"/> is either not connected or not authenticated.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<UniqueId[]> AppendAsync (MimeMessage[] messages, MessageFlags[] flags, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (messages == null)
				throw new ArgumentNullException ("messages");

			for (int i = 0; i < messages.Length; i++) {
				if (messages[i] == null)
					throw new ArgumentException ("One or more of the messages is null.");
			}

			if (flags == null)
				throw new ArgumentNullException ("flags");

			if (messages.Length != flags.Length)
				throw new ArgumentException ("The number of messages and the number of flags must be equal.");

			return Task.Factory.StartNew (() => {
				return Append (messages, flags, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Append the specified messages to the folder.
		/// </summary>
		/// <remarks>
		/// Appends the specified messages to the folder.
		/// </remarks>
		/// <returns>The UIDs of the appended messages, if available; otherwise, <c>null</c>.</returns>
		/// <param name="messages">The array of messages to append to the folder.</param>
		/// <param name="flags">The message flags to use for each of the messages.</param>
		/// <param name="dates">The received dates to use for each of the messages.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="messages"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="flags"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="dates"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para>One or more of the <paramref name="messages"/> is null.</para>
		/// <para>-or-</para>
		/// <para>The number of messages, flags, and dates do not match.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="IMailStore"/> is either not connected or not authenticated.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract UniqueId[] Append (MimeMessage[] messages, MessageFlags[] flags, DateTimeOffset[] dates, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously append the specified messages to the folder.
		/// </summary>
		/// <remarks>
		/// Asynchronously appends the specified messages to the folder.
		/// </remarks>
		/// <returns>The UIDs of the appended messages, if available; otherwise, <c>null</c>.</returns>
		/// <param name="messages">The array of messages to append to the folder.</param>
		/// <param name="flags">The message flags to use for each of the messages.</param>
		/// <param name="dates">The received dates to use for each of the messages.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="messages"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="flags"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="dates"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para>One or more of the <paramref name="messages"/> is null.</para>
		/// <para>-or-</para>
		/// <para>The number of messages, flags, and dates do not match.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// The <see cref="IMailStore"/> is either not connected or not authenticated.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<UniqueId[]> AppendAsync (MimeMessage[] messages, MessageFlags[] flags, DateTimeOffset[] dates, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (messages == null)
				throw new ArgumentNullException ("messages");

			for (int i = 0; i < messages.Length; i++) {
				if (messages[i] == null)
					throw new ArgumentException ("One or more of the messages is null.");
			}

			if (flags == null)
				throw new ArgumentNullException ("flags");

			if (dates == null)
				throw new ArgumentNullException ("dates");

			if (messages.Length != flags.Length || messages.Length != dates.Length)
				throw new ArgumentException ("The number of messages, the number of flags, and the number of dates must be equal.");

			return Task.Factory.StartNew (() => {
				return Append (messages, flags, dates, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Copy the specified messages to the destination folder.
		/// </summary>
		/// <remarks>
		/// Copies the specified messages to the destination folder.
		/// </remarks>
		/// <returns>The UIDs of the messages in the destination folder, if available; otherwise, <c>null</c>.</returns>
		/// <param name="uids">The UIDs of the messages to copy.</param>
		/// <param name="destination">The destination folder.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="uids"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="destination"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para>One or more of the <paramref name="uids"/> is invalid.</para>
		/// <para>-or-</para>
		/// <para>The destination folder does not belong to the <see cref="IMailStore"/>.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// The mail store does not support the UIDPLUS extension.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract UniqueId[] CopyTo (UniqueId[] uids, IMailFolder destination, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously copy the specified messages to the destination folder.
		/// </summary>
		/// <remarks>
		/// Asynchronously copies the specified messages to the destination folder.
		/// </remarks>
		/// <returns>The UIDs of the messages in the destination folder, if available; otherwise, <c>null</c>.</returns>
		/// <param name="uids">The UIDs of the messages to copy.</param>
		/// <param name="destination">The destination folder.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="uids"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="destination"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="uids"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="uids"/> is invalid.</para>
		/// <para>-or-</para>
		/// <para>The destination folder does not belong to the <see cref="IMailStore"/>.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// The mail store does not support the UIDPLUS extension.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<UniqueId[]> CopyToAsync (UniqueId[] uids, IMailFolder destination, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (uids == null)
				throw new ArgumentNullException ("uids");

			if (uids.Length == 0)
				throw new ArgumentException ("No uids were specified.", "uids");

			if (destination == null)
				throw new ArgumentNullException ("destination");

			return Task.Factory.StartNew (() => {
				return CopyTo (uids, destination, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Move the specified messages to the destination folder.
		/// </summary>
		/// <remarks>
		/// Moves the specified messages to the destination folder.
		/// </remarks>
		/// <returns>The UIDs of the messages in the destination folder, if available; otherwise, <c>null</c>.</returns>
		/// <param name="uids">The UIDs of the messages to copy.</param>
		/// <param name="destination">The destination folder.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="uids"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="destination"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="uids"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="uids"/> is invalid.</para>
		/// <para>-or-</para>
		/// <para>The destination folder does not belong to the <see cref="IMailStore"/>.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open in read-write mode.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// The mail store does not support the UIDPLUS extension.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract UniqueId[] MoveTo (UniqueId[] uids, IMailFolder destination, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously move the specified messages to the destination folder.
		/// </summary>
		/// <remarks>
		/// Asynchronously moves the specified messages to the destination folder.
		/// </remarks>
		/// <returns>The UIDs of the messages in the destination folder, if available; otherwise, <c>null</c>.</returns>
		/// <param name="uids">The UIDs of the messages to copy.</param>
		/// <param name="destination">The destination folder.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="uids"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="destination"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="uids"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="uids"/> is invalid.</para>
		/// <para>-or-</para>
		/// <para>The destination folder does not belong to the <see cref="IMailStore"/>.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open in read-write mode.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// The mail store does not support the UIDPLUS extension.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<UniqueId[]> MoveToAsync (UniqueId[] uids, IMailFolder destination, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (uids == null)
				throw new ArgumentNullException ("uids");

			if (uids.Length == 0)
				throw new ArgumentException ("No uids were specified.", "uids");

			if (destination == null)
				throw new ArgumentNullException ("destination");

			return Task.Factory.StartNew (() => {
				return MoveTo (uids, destination, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Copy the specified messages to the destination folder.
		/// </summary>
		/// <remarks>
		/// Copies the specified messages to the destination folder.
		/// </remarks>
		/// <param name="indexes">The indexes of the messages to copy.</param>
		/// <param name="destination">The destination folder.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="indexes"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="destination"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="indexes"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="indexes"/> is invalid.</para>
		/// <para>-or-</para>
		/// <para>The destination folder does not belong to the <see cref="IMailStore"/>.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract void CopyTo (int[] indexes, IMailFolder destination, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously copy the specified messages to the destination folder.
		/// </summary>
		/// <remarks>
		/// Asynchronously copies the specified messages to the destination folder.
		/// </remarks>
		/// <param name="indexes">The indexes of the messages to copy.</param>
		/// <param name="destination">The destination folder.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="indexes"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="destination"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="indexes"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="indexes"/> is invalid.</para>
		/// <para>-or-</para>
		/// <para>The destination folder does not belong to the <see cref="IMailStore"/>.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task CopyToAsync (int[] indexes, IMailFolder destination, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (indexes == null)
				throw new ArgumentNullException ("indexes");

			if (destination == null)
				throw new ArgumentNullException ("destination");

			return Task.Factory.StartNew (() => {
				CopyTo (indexes, destination, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Move the specified messages to the destination folder.
		/// </summary>
		/// <remarks>
		/// Moves the specified messages to the destination folder.
		/// </remarks>
		/// <param name="indexes">The indexes of the messages to copy.</param>
		/// <param name="destination">The destination folder.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="indexes"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="destination"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="indexes"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="indexes"/> is invalid.</para>
		/// <para>-or-</para>
		/// <para>The destination folder does not belong to the <see cref="IMailStore"/>.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open in read-write mode.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract void MoveTo (int[] indexes, IMailFolder destination, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously move the specified messages to the destination folder.
		/// </summary>
		/// <remarks>
		/// Asynchronously moves the specified messages to the destination folder.
		/// </remarks>
		/// <param name="indexes">The indexes of the messages to copy.</param>
		/// <param name="destination">The destination folder.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="indexes"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="destination"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="indexes"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="indexes"/> is invalid.</para>
		/// <para>-or-</para>
		/// <para>The destination folder does not belong to the <see cref="IMailStore"/>.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open in read-write mode.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task MoveToAsync (int[] indexes, IMailFolder destination, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (indexes == null)
				throw new ArgumentNullException ("indexes");

			if (destination == null)
				throw new ArgumentNullException ("destination");

			return Task.Factory.StartNew (() => {
				MoveTo (indexes, destination, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Fetch the message summaries for the specified message UIDs.
		/// </summary>
		/// <remarks>
		/// Fetches the message summaries for the specified message UIDs.
		/// </remarks>
		/// <returns>An enumeration of summaries for the requested messages.</returns>
		/// <param name="uids">The UIDs.</param>
		/// <param name="items">The message summary items to fetch.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="uids"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="items"/> is empty.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="uids"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="uids"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract IEnumerable<MessageSummary> Fetch (UniqueId[] uids, MessageSummaryItems items, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously fetch the message summaries for the specified message UIDs.
		/// </summary>
		/// <remarks>
		/// Asynchronously fetches the message summaries for the specified message UIDs.
		/// </remarks>
		/// <returns>An enumeration of summaries for the requested messages.</returns>
		/// <param name="uids">The UIDs.</param>
		/// <param name="items">The message summary items to fetch.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="uids"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="items"/> is empty.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="uids"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="uids"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<IEnumerable<MessageSummary>> FetchAsync (UniqueId[] uids, MessageSummaryItems items, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (uids == null)
				throw new ArgumentNullException ("uids");

			if (uids.Length == 0)
				throw new ArgumentException ("No uids were specified.", "uids");

			if (items == MessageSummaryItems.None)
				throw new ArgumentOutOfRangeException ("items");

			return Task.Factory.StartNew (() => {
				return Fetch (uids, items, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Fetch the message summaries for the specified message UIDs that have a higher mod-sequence value than the one specified.
		/// </summary>
		/// <remarks>
		/// <para>If the mail store supports quick resynchronization and the application has
		/// enabled this feature via <see cref="MailStore.EnableQuickResync(CancellationToken)"/>,
		/// then this method will emit <see cref="MessagesVanished"/> events for messages that have vanished
		/// since the specified mod-sequence value.</para>
		/// </remarks>
		/// <returns>An enumeration of summaries for the requested messages.</returns>
		/// <param name="uids">The UIDs.</param>
		/// <param name="modseq">The mod-sequence value.</param>
		/// <param name="items">The message summary items to fetch.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="uids"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="items"/> is empty.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="uids"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="uids"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// The <see cref="IMailStore"/> does not support mod-sequences.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract IEnumerable<MessageSummary> Fetch (UniqueId[] uids, ulong modseq, MessageSummaryItems items, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously fetch the message summaries for the specified message UIDs that have a higher mod-sequence value than the one specified.
		/// </summary>
		/// <remarks>
		/// <para>If the mail store supports quick resynchronization and the application has
		/// enabled this feature via <see cref="MailStore.EnableQuickResync(CancellationToken)"/>,
		/// then this method will emit <see cref="MessagesVanished"/> events for messages that have vanished
		/// since the specified mod-sequence value.</para>
		/// </remarks>
		/// <returns>An enumeration of summaries for the requested messages.</returns>
		/// <param name="uids">The UIDs.</param>
		/// <param name="modseq">The mod-sequence value.</param>
		/// <param name="items">The message summary items to fetch.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="uids"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="items"/> is empty.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="uids"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="uids"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// The <see cref="IMailStore"/> does not support mod-sequences.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<IEnumerable<MessageSummary>> FetchAsync (UniqueId[] uids, ulong modseq, MessageSummaryItems items, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (uids == null)
				throw new ArgumentNullException ("uids");

			if (uids.Length == 0)
				throw new ArgumentException ("No uids were specified.", "uids");

			if (items == MessageSummaryItems.None)
				throw new ArgumentOutOfRangeException ("items");

			return Task.Factory.StartNew (() => {
				return Fetch (uids, modseq, items, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Fetch the message summaries for the messages between the two UIDs, inclusive.
		/// </summary>
		/// <remarks>
		/// Fetches the message summaries for the messages between the two UIDs, inclusive.
		/// </remarks>
		/// <returns>An enumeration of summaries for the requested messages.</returns>
		/// <param name="min">The minimum UID.</param>
		/// <param name="max">The maximum UID, or <c>null</c> to specify no upper bound.</param>
		/// <param name="items">The message summary items to fetch.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="min"/> is invalid.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="items"/> is empty.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract IEnumerable<MessageSummary> Fetch (UniqueId min, UniqueId? max, MessageSummaryItems items, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously fetch the message summaries for the messages between the two UIDs, inclusive.
		/// </summary>
		/// <remarks>
		/// Asynchronously fetches the message summaries for the messages between the two UIDs, inclusive.
		/// </remarks>
		/// <returns>An enumeration of summaries for the requested messages.</returns>
		/// <param name="min">The minimum UID.</param>
		/// <param name="max">The maximum UID, or <c>null</c> to specify no upper bound.</param>
		/// <param name="items">The message summary items to fetch.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="min"/> is invalid.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="items"/> is empty.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<IEnumerable<MessageSummary>> FetchAsync (UniqueId min, UniqueId? max, MessageSummaryItems items, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (items == MessageSummaryItems.None)
				throw new ArgumentOutOfRangeException ("items");

			return Task.Factory.StartNew (() => {
				return Fetch (min, max, items, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Fetch the message summaries for the messages between the two UIDs (inclusive) that have a higher mod-sequence value than the one specified.
		/// </summary>
		/// <remarks>
		/// <para>If the mail store supports the quick resynchronization feature and the application has
		/// enabled this feature via <see cref="MailStore.EnableQuickResync(CancellationToken)"/>,
		/// then this method will emit <see cref="MessagesVanished"/> events for messages that have vanished
		/// since the specified mod-sequence value.</para>
		/// </remarks>
		/// <returns>An enumeration of summaries for the requested messages.</returns>
		/// <param name="min">The minimum UID.</param>
		/// <param name="max">The maximum UID.</param>
		/// <param name="modseq">The mod-sequence value.</param>
		/// <param name="items">The message summary items to fetch.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="min"/> is invalid.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="items"/> is empty.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// The <see cref="MailFolder"/> does not support mod-sequences.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract IEnumerable<MessageSummary> Fetch (UniqueId min, UniqueId? max, ulong modseq, MessageSummaryItems items, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously fetch the message summaries for the messages between the two UIDs (inclusive) that have a higher mod-sequence value than the one specified.
		/// </summary>
		/// <remarks>
		/// <para>If the mail store supports the quick resynchronization feature and the application has
		/// enabled this feature via <see cref="MailStore.EnableQuickResync(CancellationToken)"/>,
		/// then this method will emit <see cref="MessagesVanished"/> events for messages that have vanished
		/// since the specified mod-sequence value.</para>
		/// </remarks>
		/// <returns>An enumeration of summaries for the requested messages.</returns>
		/// <param name="min">The minimum UID.</param>
		/// <param name="max">The maximum UID.</param>
		/// <param name="modseq">The mod-sequence value.</param>
		/// <param name="items">The message summary items to fetch.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="min"/> is invalid.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="items"/> is empty.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// The <see cref="MailFolder"/> does not support mod-sequences.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<IEnumerable<MessageSummary>> FetchAsync (UniqueId min, UniqueId? max, ulong modseq, MessageSummaryItems items, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (items == MessageSummaryItems.None)
				throw new ArgumentOutOfRangeException ("items");

			return Task.Factory.StartNew (() => {
				return Fetch (min, max, modseq, items, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Fetch the message summaries for the specified message indexes.
		/// </summary>
		/// <remarks>
		/// Fetches the message summaries for the specified message indexes.
		/// </remarks>
		/// <returns>An enumeration of summaries for the requested messages.</returns>
		/// <param name="indexes">The indexes.</param>
		/// <param name="items">The message summary items to fetch.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="indexes"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="items"/> is empty.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="indexes"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="indexes"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract IEnumerable<MessageSummary> Fetch (int[] indexes, MessageSummaryItems items, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously fetch the message summaries for the specified message indexes.
		/// </summary>
		/// <remarks>
		/// Asynchronously fetches the message summaries for the specified message indexes.
		/// </remarks>
		/// <returns>An enumeration of summaries for the requested messages.</returns>
		/// <param name="indexes">The indexes.</param>
		/// <param name="items">The message summary items to fetch.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="indexes"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="items"/> is empty.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="indexes"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="indexes"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<IEnumerable<MessageSummary>> FetchAsync (int[] indexes, MessageSummaryItems items, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (indexes == null)
				throw new ArgumentNullException ("indexes");

			if (items == MessageSummaryItems.None)
				throw new ArgumentOutOfRangeException ("items");

			return Task.Factory.StartNew (() => {
				return Fetch (indexes, items, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Fetch the message summaries for the specified message indexes that have a higher mod-sequence value than the one specified.
		/// </summary>
		/// <remarks>
		/// Fetches the message summaries for the specified message indexes that have a higher mod-sequence value than the one specified.
		/// </remarks>
		/// <returns>An enumeration of summaries for the requested messages.</returns>
		/// <param name="indexes">The indexes.</param>
		/// <param name="modseq">The mod-sequence value.</param>
		/// <param name="items">The message summary items to fetch.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="indexes"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="items"/> is empty.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="indexes"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="indexes"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// The <see cref="MailFolder"/> does not support mod-sequences.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract IEnumerable<MessageSummary> Fetch (int[] indexes, ulong modseq, MessageSummaryItems items, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously fetch the message summaries for the specified message indexes that have a higher mod-sequence value than the one specified.
		/// </summary>
		/// <remarks>
		/// Asynchronously fetches the message summaries for the specified message indexes that have a higher mod-sequence value than the one specified.
		/// </remarks>
		/// <returns>An enumeration of summaries for the requested messages.</returns>
		/// <param name="indexes">The indexes.</param>
		/// <param name="modseq">The mod-sequence value.</param>
		/// <param name="items">The message summary items to fetch.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="indexes"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="items"/> is empty.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="indexes"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="indexes"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// The <see cref="MailFolder"/> does not support mod-sequences.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<IEnumerable<MessageSummary>> FetchAsync (int[] indexes, ulong modseq, MessageSummaryItems items, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (indexes == null)
				throw new ArgumentNullException ("indexes");

			if (items == MessageSummaryItems.None)
				throw new ArgumentOutOfRangeException ("items");

			return Task.Factory.StartNew (() => {
				return Fetch (indexes, modseq, items, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Fetch the message summaries for the messages between the two indexes, inclusive.
		/// </summary>
		/// <remarks>
		/// Fetches the message summaries for the messages between the two indexes, inclusive.
		/// </remarks>
		/// <returns>An enumeration of summaries for the requested messages.</returns>
		/// <param name="min">The minimum index.</param>
		/// <param name="max">The maximum index, or <c>-1</c> to specify no upper bound.</param>
		/// <param name="items">The message summary items to fetch.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <para><paramref name="min"/> is out of range.</para>
		/// <para>-or-</para>
		/// <para><paramref name="max"/> is out of range.</para>
		/// <para>-or-</para>
		/// <para><paramref name="items"/> is empty.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract IEnumerable<MessageSummary> Fetch (int min, int max, MessageSummaryItems items, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously fetch the message summaries for the messages between the two indexes, inclusive.
		/// </summary>
		/// <remarks>
		/// Asynchronously fetches the message summaries for the messages between the two indexes, inclusive.
		/// </remarks>
		/// <returns>An enumeration of summaries for the requested messages.</returns>
		/// <param name="min">The minimum index.</param>
		/// <param name="max">The maximum index, or <c>-1</c> to specify no upper bound.</param>
		/// <param name="items">The message summary items to fetch.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <para><paramref name="min"/> is out of range.</para>
		/// <para>-or-</para>
		/// <para><paramref name="max"/> is out of range.</para>
		/// <para>-or-</para>
		/// <para><paramref name="items"/> is empty.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<IEnumerable<MessageSummary>> FetchAsync (int min, int max, MessageSummaryItems items, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (min < 0)
				throw new ArgumentOutOfRangeException ("min");

			if (max != -1 && max < min)
				throw new ArgumentOutOfRangeException ("max");

			if (items == MessageSummaryItems.None)
				throw new ArgumentOutOfRangeException ("items");

			return Task.Factory.StartNew (() => {
				return Fetch (min, max, items, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Fetch the message summaries for the messages between the two indexes (inclusive) that have a higher mod-sequence value than the one specified.
		/// </summary>
		/// <remarks>
		/// Fetches the message summaries for the messages between the two indexes (inclusive) that have a higher mod-sequence value than the one specified.
		/// </remarks>
		/// <returns>An enumeration of summaries for the requested messages.</returns>
		/// <param name="min">The minimum index.</param>
		/// <param name="max">The maximum index, or <c>-1</c> to specify no upper bound.</param>
		/// <param name="modseq">The mod-sequence value.</param>
		/// <param name="items">The message summary items to fetch.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <para><paramref name="min"/> is out of range.</para>
		/// <para>-or-</para>
		/// <para><paramref name="max"/> is out of range.</para>
		/// <para>-or-</para>
		/// <para><paramref name="items"/> is empty.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// The <see cref="MailFolder"/> does not support mod-sequences.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract IEnumerable<MessageSummary> Fetch (int min, int max, ulong modseq, MessageSummaryItems items, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously fetch the message summaries for the messages between the two indexes (inclusive) that have a higher mod-sequence value than the one specified.
		/// </summary>
		/// <remarks>
		/// Asynchronously fetches the message summaries for the messages between the two indexes (inclusive) that have a higher mod-sequence value than the one specified.
		/// </remarks>
		/// <returns>An enumeration of summaries for the requested messages.</returns>
		/// <param name="min">The minimum index.</param>
		/// <param name="max">The maximum index, or <c>-1</c> to specify no upper bound.</param>
		/// <param name="modseq">The mod-sequence value.</param>
		/// <param name="items">The message summary items to fetch.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <para><paramref name="min"/> is out of range.</para>
		/// <para>-or-</para>
		/// <para><paramref name="max"/> is out of range.</para>
		/// <para>-or-</para>
		/// <para><paramref name="items"/> is empty.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// The <see cref="MailFolder"/> does not support mod-sequences.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<IEnumerable<MessageSummary>> FetchAsync (int min, int max, ulong modseq, MessageSummaryItems items, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (min < 0)
				throw new ArgumentOutOfRangeException ("min");

			if (max != -1 && max < min)
				throw new ArgumentOutOfRangeException ("max");

			if (items == MessageSummaryItems.None)
				throw new ArgumentOutOfRangeException ("items");

			return Task.Factory.StartNew (() => {
				return Fetch (min, max, modseq, items, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Get the specified message.
		/// </summary>
		/// <remarks>
		/// Gets the specified message.
		/// </remarks>
		/// <returns>The message.</returns>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="uid"/> is invalid.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract MimeMessage GetMessage (UniqueId uid, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously get the specified message.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets the specified message.
		/// </remarks>
		/// <returns>The message.</returns>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="uid"/> is invalid.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<MimeMessage> GetMessageAsync (UniqueId uid, CancellationToken cancellationToken = default (CancellationToken))
		{
			return Task.Factory.StartNew (() => {
				return GetMessage (uid, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Get the specified message.
		/// </summary>
		/// <remarks>
		/// Gets the specified message.
		/// </remarks>
		/// <returns>The message.</returns>
		/// <param name="index">The index of the message.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="index"/> is out of range.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract MimeMessage GetMessage (int index, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously get the specified message.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets the specified message.
		/// </remarks>
		/// <returns>The message.</returns>
		/// <param name="index">The index of the message.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="index"/> is out of range.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<MimeMessage> GetMessageAsync (int index, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (index < 0 || index >= Count)
				throw new ArgumentOutOfRangeException ("index");

			return Task.Factory.StartNew (() => {
				return GetMessage (index, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Get the specified body part.
		/// </summary>
		/// <remarks>
		/// Gets the specified body part.
		/// </remarks>
		/// <returns>The body part.</returns>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="part">The body part.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="part"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="uid"/> is invalid.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract MimeEntity GetBodyPart (UniqueId uid, BodyPart part, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously get the specified body part.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets the specified body part.
		/// </remarks>
		/// <returns>The body part.</returns>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="part">The body part.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="part"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="uid"/> is invalid.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<MimeEntity> GetBodyPartAsync (UniqueId uid, BodyPart part, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (part == null)
				throw new ArgumentNullException ("part");

			return Task.Factory.StartNew (() => {
				return GetBodyPart (uid, part, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Get the specified body part.
		/// </summary>
		/// <remarks>
		/// Gets the specified body part.
		/// </remarks>
		/// <returns>The body part.</returns>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="part">The body part.</param>
		/// <param name="headersOnly"><c>true</c> if only the headers should be downloaded; otherwise, <c>false</c>></param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="part"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="uid"/> is invalid.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract MimeEntity GetBodyPart (UniqueId uid, BodyPart part, bool headersOnly, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously get the specified body part.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets the specified body part.
		/// </remarks>
		/// <returns>The body part.</returns>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="part">The body part.</param>
		/// <param name="headersOnly"><c>true</c> if only the headers should be downloaded; otherwise, <c>false</c>></param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="part"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="uid"/> is invalid.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<MimeEntity> GetBodyPartAsync (UniqueId uid, BodyPart part, bool headersOnly, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (part == null)
				throw new ArgumentNullException ("part");

			return Task.Factory.StartNew (() => {
				return GetBodyPart (uid, part, headersOnly, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Get the specified body part.
		/// </summary>
		/// <remarks>
		/// Gets the specified body part.
		/// </remarks>
		/// <returns>The body part.</returns>
		/// <param name="index">The index of the message.</param>
		/// <param name="part">The body part.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="part"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="index"/> is out of range.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract MimeEntity GetBodyPart (int index, BodyPart part, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously get the specified body part.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets the specified body part.
		/// </remarks>
		/// <returns>The body part.</returns>
		/// <param name="index">The index of the message.</param>
		/// <param name="part">The body part.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="part"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="index"/> is out of range.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<MimeEntity> GetBodyPartAsync (int index, BodyPart part, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (index < 0 || index >= Count)
				throw new ArgumentOutOfRangeException ("index");

			if (part == null)
				throw new ArgumentNullException ("part");

			return Task.Factory.StartNew (() => {
				return GetBodyPart (index, part, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Get the specified body part.
		/// </summary>
		/// <remarks>
		/// Gets the specified body part.
		/// </remarks>
		/// <returns>The body part.</returns>
		/// <param name="index">The index of the message.</param>
		/// <param name="part">The body part.</param>
		/// <param name="headersOnly"><c>true</c> if only the headers should be downloaded; otherwise, <c>false</c>></param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="part"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="index"/> is out of range.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract MimeEntity GetBodyPart (int index, BodyPart part, bool headersOnly, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously get the specified body part.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets the specified body part.
		/// </remarks>
		/// <returns>The body part.</returns>
		/// <param name="index">The index of the message.</param>
		/// <param name="part">The body part.</param>
		/// <param name="headersOnly"><c>true</c> if only the headers should be downloaded; otherwise, <c>false</c>></param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="part"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="index"/> is out of range.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<MimeEntity> GetBodyPartAsync (int index, BodyPart part, bool headersOnly, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (index < 0 || index >= Count)
				throw new ArgumentOutOfRangeException ("index");

			if (part == null)
				throw new ArgumentNullException ("part");

			return Task.Factory.StartNew (() => {
				return GetBodyPart (index, part, headersOnly, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Get a substream of the specified message.
		/// </summary>
		/// <remarks>
		/// Gets a substream of the message. If the starting offset is beyond
		/// the end of the message, an empty stream is returned. If the number of
		/// bytes desired extends beyond the end of the message, a truncated stream
		/// will be returned.
		/// </remarks>
		/// <returns>The stream.</returns>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="offset">The starting offset of the first desired byte.</param>
		/// <param name="count">The number of bytes desired.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="uid"/> is invalid.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <para><paramref name="offset"/> is negative.</para>
		/// <para>-or-</para>
		/// <para><paramref name="count"/> is negative.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract Stream GetStream (UniqueId uid, int offset, int count, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously get a substream of the specified message.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets a substream of the message. If the starting offset is beyond
		/// the end of the message, an empty stream is returned. If the number of
		/// bytes desired extends beyond the end of the message, a truncated stream
		/// will be returned.
		/// </remarks>
		/// <returns>The stream.</returns>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="offset">The starting offset of the first desired byte.</param>
		/// <param name="count">The number of bytes desired.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="uid"/> is invalid.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <para><paramref name="offset"/> is negative.</para>
		/// <para>-or-</para>
		/// <para><paramref name="count"/> is negative.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<Stream> GetStreamAsync (UniqueId uid, int offset, int count, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (offset < 0)
				throw new ArgumentOutOfRangeException ("offset");

			if (count < 0)
				throw new ArgumentOutOfRangeException ("count");

			return Task.Factory.StartNew (() => {
				return GetStream (uid, offset, count, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Get a substream of the specified message.
		/// </summary>
		/// <remarks>
		/// Gets a substream of the message. If the starting offset is beyond
		/// the end of the message, an empty stream is returned. If the number of
		/// bytes desired extends beyond the end of the message, a truncated stream
		/// will be returned.
		/// </remarks>
		/// <returns>The stream.</returns>
		/// <param name="index">The index of the message.</param>
		/// <param name="offset">The starting offset of the first desired byte.</param>
		/// <param name="count">The number of bytes desired.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <para><paramref name="index"/> is out of range.</para>
		/// <para>-or-</para>
		/// <para><paramref name="offset"/> is negative.</para>
		/// <para>-or-</para>
		/// <para><paramref name="count"/> is negative.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract Stream GetStream (int index, int offset, int count, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously get a substream of the specified message.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets a substream of the message. If the starting offset is beyond
		/// the end of the message, an empty stream is returned. If the number of
		/// bytes desired extends beyond the end of the message, a truncated stream
		/// will be returned.
		/// </remarks>
		/// <returns>The stream.</returns>
		/// <param name="index">The index of the message.</param>
		/// <param name="offset">The starting offset of the first desired byte.</param>
		/// <param name="count">The number of bytes desired.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <para><paramref name="index"/> is out of range.</para>
		/// <para>-or-</para>
		/// <para><paramref name="offset"/> is negative.</para>
		/// <para>-or-</para>
		/// <para><paramref name="count"/> is negative.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<Stream> GetStreamAsync (int index, int offset, int count, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (index < 0 || index >= Count)
				throw new ArgumentOutOfRangeException ("index");

			if (offset < 0)
				throw new ArgumentOutOfRangeException ("offset");

			if (count < 0)
				throw new ArgumentOutOfRangeException ("count");

			return Task.Factory.StartNew (() => {
				return GetStream (index, offset, count, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Get a substream of the specified body part.
		/// </summary>
		/// <remarks>
		/// Gets a substream of the body part. If the starting offset is beyond
		/// the end of the body part, an empty stream is returned. If the number of
		/// bytes desired extends beyond the end of the body part, a truncated stream
		/// will be returned.
		/// </remarks>
		/// <returns>The stream.</returns>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="part">The desired body part.</param>
		/// <param name="offset">The starting offset of the first desired byte.</param>
		/// <param name="count">The number of bytes desired.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="uid"/> is invalid.
		/// </exception>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="part"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <para><paramref name="offset"/> is negative.</para>
		/// <para>-or-</para>
		/// <para><paramref name="count"/> is negative.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract Stream GetStream (UniqueId uid, BodyPart part, int offset, int count, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously get a substream of the specified body part.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets a substream of the body part. If the starting offset is beyond
		/// the end of the body part, an empty stream is returned. If the number of
		/// bytes desired extends beyond the end of the body part, a truncated stream
		/// will be returned.
		/// </remarks>
		/// <returns>The stream.</returns>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="part">The desired body part.</param>
		/// <param name="offset">The starting offset of the first desired byte.</param>
		/// <param name="count">The number of bytes desired.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="uid"/> is invalid.
		/// </exception>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="part"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <para><paramref name="offset"/> is negative.</para>
		/// <para>-or-</para>
		/// <para><paramref name="count"/> is negative.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<Stream> GetStreamAsync (UniqueId uid, BodyPart part, int offset, int count, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (part == null)
				throw new ArgumentNullException ("part");

			if (offset < 0)
				throw new ArgumentOutOfRangeException ("offset");

			if (count < 0)
				throw new ArgumentOutOfRangeException ("count");

			return Task.Factory.StartNew (() => {
				return GetStream (uid, part, offset, count, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Get a substream of the specified body part.
		/// </summary>
		/// <remarks>
		/// Gets a substream of the body part. If the starting offset is beyond
		/// the end of the body part, an empty stream is returned. If the number of
		/// bytes desired extends beyond the end of the body part, a truncated stream
		/// will be returned.
		/// </remarks>
		/// <returns>The stream.</returns>
		/// <param name="index">The index of the message.</param>
		/// <param name="part">The desired body part.</param>
		/// <param name="offset">The starting offset of the first desired byte.</param>
		/// <param name="count">The number of bytes desired.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="part"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <para><paramref name="index"/> is out of range.</para>
		/// <para>-or-</para>
		/// <para><paramref name="offset"/> is negative.</para>
		/// <para>-or-</para>
		/// <para><paramref name="count"/> is negative.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract Stream GetStream (int index, BodyPart part, int offset, int count, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously get a substream of the specified body part.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets a substream of the body part. If the starting offset is beyond
		/// the end of the body part, an empty stream is returned. If the number of
		/// bytes desired extends beyond the end of the body part, a truncated stream
		/// will be returned.
		/// </remarks>
		/// <returns>The stream.</returns>
		/// <param name="index">The index of the message.</param>
		/// <param name="part">The desired body part.</param>
		/// <param name="offset">The starting offset of the first desired byte.</param>
		/// <param name="count">The number of bytes desired.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="part"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <para><paramref name="index"/> is out of range.</para>
		/// <para>-or-</para>
		/// <para><paramref name="offset"/> is negative.</para>
		/// <para>-or-</para>
		/// <para><paramref name="count"/> is negative.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<Stream> GetStreamAsync (int index, BodyPart part, int offset, int count, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (index < 0 || index >= Count)
				throw new ArgumentOutOfRangeException ("index");

			if (part == null)
				throw new ArgumentNullException ("part");

			if (offset < 0)
				throw new ArgumentOutOfRangeException ("offset");

			if (count < 0)
				throw new ArgumentOutOfRangeException ("count");

			return Task.Factory.StartNew (() => {
				return GetStream (index, part, offset, count, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Add a set of flags to the specified messages.
		/// </summary>
		/// <remarks>
		/// Adds a set of flags to the specified messages.
		/// </remarks>
		/// <param name="uids">The UIDs of the messages.</param>
		/// <param name="flags">The message flags to add.</param>
		/// <param name="silent">If set to <c>true</c>, no <see cref="MessageFlagsChanged"/> events will be emitted.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="uids"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="uids"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="uids"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open in read-write mode.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract void AddFlags (UniqueId[] uids, MessageFlags flags, bool silent, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously add a set of flags to the specified messages.
		/// </summary>
		/// <remarks>
		/// Asynchronously adds a set of flags to the specified messages.
		/// </remarks>
		/// <param name="uids">The UIDs of the messages.</param>
		/// <param name="flags">The message flags to add.</param>
		/// <param name="silent">If set to <c>true</c>, no <see cref="MessageFlagsChanged"/> events will be emitted.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="uids"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="uids"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="uids"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open in read-write mode.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task AddFlagsAsync (UniqueId[] uids, MessageFlags flags, bool silent, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (uids == null)
				throw new ArgumentNullException ("uids");

			if (uids.Length == 0)
				throw new ArgumentException ("No uids were specified.", "uids");

			return Task.Factory.StartNew (() => {
				AddFlags (uids, flags, silent, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Remove a set of flags from the specified messages.
		/// </summary>
		/// <remarks>
		/// Removes a set of flags from the specified messages.
		/// </remarks>
		/// <param name="uids">The UIDs of the messages.</param>
		/// <param name="flags">The message flags to remove.</param>
		/// <param name="silent">If set to <c>true</c>, no <see cref="MessageFlagsChanged"/> events will be emitted.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="uids"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="uids"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="uids"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open in read-write mode.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract void RemoveFlags (UniqueId[] uids, MessageFlags flags, bool silent, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously remove a set of flags from the specified messages.
		/// </summary>
		/// <remarks>
		/// Asynchronously removes a set of flags from the specified messages.
		/// </remarks>
		/// <param name="uids">The UIDs of the messages.</param>
		/// <param name="flags">The message flags to remove.</param>
		/// <param name="silent">If set to <c>true</c>, no <see cref="MessageFlagsChanged"/> events will be emitted.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="uids"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="uids"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="uids"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open in read-write mode.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task RemoveFlagsAsync (UniqueId[] uids, MessageFlags flags, bool silent, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (uids == null)
				throw new ArgumentNullException ("uids");

			if (uids.Length == 0)
				throw new ArgumentException ("No uids were specified.", "uids");

			return Task.Factory.StartNew (() => {
				RemoveFlags (uids, flags, silent, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Set the flags of the specified messages.
		/// </summary>
		/// <remarks>
		/// Sets the flags of the specified messages.
		/// </remarks>
		/// <param name="uids">The UIDs of the messages.</param>
		/// <param name="flags">The message flags to set.</param>
		/// <param name="silent">If set to <c>true</c>, no <see cref="MessageFlagsChanged"/> events will be emitted.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="uids"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="uids"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="uids"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open in read-write mode.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract void SetFlags (UniqueId[] uids, MessageFlags flags, bool silent, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously set the flags of the specified messages.
		/// </summary>
		/// <remarks>
		/// Asynchronously sets the flags of the specified messages.
		/// </remarks>
		/// <param name="uids">The UIDs of the messages.</param>
		/// <param name="flags">The message flags to set.</param>
		/// <param name="silent">If set to <c>true</c>, no <see cref="MessageFlagsChanged"/> events will be emitted.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="uids"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="uids"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="uids"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open in read-write mode.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task SetFlagsAsync (UniqueId[] uids, MessageFlags flags, bool silent, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (uids == null)
				throw new ArgumentNullException ("uids");

			if (uids.Length == 0)
				throw new ArgumentException ("No uids were specified.", "uids");

			return Task.Factory.StartNew (() => {
				SetFlags (uids, flags, silent, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Add a set of flags to the specified messages only if their mod-sequence value is less than the specified value.
		/// </summary>
		/// <remarks>
		/// Adds a set of flags to the specified messages only if their mod-sequence value is less than the specified value.
		/// </remarks>
		/// <returns>The unique IDs of the messages that were not updated.</returns>
		/// <param name="uids">The UIDs of the messages.</param>
		/// <param name="modseq">The mod-sequence value.</param>
		/// <param name="flags">The message flags to add.</param>
		/// <param name="silent">If set to <c>true</c>, no <see cref="MessageFlagsChanged"/> events will be emitted.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="uids"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="uids"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="uids"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open in read-write mode.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// The <see cref="MailFolder"/> does not support mod-sequences.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract UniqueId[] AddFlags (UniqueId[] uids, ulong modseq, MessageFlags flags, bool silent, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously add a set of flags to the specified messages only if their mod-sequence value is less than the specified value.
		/// </summary>
		/// <remarks>
		/// Asynchronously adds a set of flags to the specified messages only if their mod-sequence value is less than the specified value.
		/// </remarks>
		/// <returns>The unique IDs of the messages that were not updated.</returns>
		/// <param name="uids">The UIDs of the messages.</param>
		/// <param name="modseq">The mod-sequence value.</param>
		/// <param name="flags">The message flags to add.</param>
		/// <param name="silent">If set to <c>true</c>, no <see cref="MessageFlagsChanged"/> events will be emitted.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="uids"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="uids"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="uids"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open in read-write mode.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// The <see cref="MailFolder"/> does not support mod-sequences.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<UniqueId[]> AddFlagsAsync (UniqueId[] uids, ulong modseq, MessageFlags flags, bool silent, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (uids == null)
				throw new ArgumentNullException ("uids");

			if (uids.Length == 0)
				throw new ArgumentException ("No uids were specified.", "uids");

			return Task.Factory.StartNew (() => {
				return AddFlags (uids, modseq, flags, silent, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Remove a set of flags from the specified messages only if their mod-sequence value is less than the specified value.
		/// </summary>
		/// <remarks>
		/// Removes a set of flags from the specified messages only if their mod-sequence value is less than the specified value.
		/// </remarks>
		/// <returns>The unique IDs of the messages that were not updated.</returns>
		/// <param name="uids">The UIDs of the messages.</param>
		/// <param name="modseq">The mod-sequence value.</param>
		/// <param name="flags">The message flags to remove.</param>
		/// <param name="silent">If set to <c>true</c>, no <see cref="MessageFlagsChanged"/> events will be emitted.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="uids"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="uids"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="uids"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open in read-write mode.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// The <see cref="MailFolder"/> does not support mod-sequences.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract UniqueId[] RemoveFlags (UniqueId[] uids, ulong modseq, MessageFlags flags, bool silent, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously remove a set of flags from the specified messages only if their mod-sequence value is less than the specified value.
		/// </summary>
		/// <remarks>
		/// Asynchronously removes a set of flags from the specified messages only if their mod-sequence value is less than the specified value.
		/// </remarks>
		/// <returns>The unique IDs of the messages that were not updated.</returns>
		/// <param name="uids">The UIDs of the messages.</param>
		/// <param name="modseq">The mod-sequence value.</param>
		/// <param name="flags">The message flags to remove.</param>
		/// <param name="silent">If set to <c>true</c>, no <see cref="MessageFlagsChanged"/> events will be emitted.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="uids"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="uids"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="uids"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open in read-write mode.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// The <see cref="MailFolder"/> does not support mod-sequences.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<UniqueId[]> RemoveFlagsAsync (UniqueId[] uids, ulong modseq, MessageFlags flags, bool silent, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (uids == null)
				throw new ArgumentNullException ("uids");

			if (uids.Length == 0)
				throw new ArgumentException ("No uids were specified.", "uids");

			return Task.Factory.StartNew (() => {
				return RemoveFlags (uids, modseq, flags, silent, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Set the flags of the specified messages only if their mod-sequence value is less than the specified value.
		/// </summary>
		/// <remarks>
		/// Sets the flags of the specified messages only if their mod-sequence value is less than the specified value.
		/// </remarks>
		/// <returns>The unique IDs of the messages that were not updated.</returns>
		/// <param name="uids">The UIDs of the messages.</param>
		/// <param name="modseq">The mod-sequence value.</param>
		/// <param name="flags">The message flags to set.</param>
		/// <param name="silent">If set to <c>true</c>, no <see cref="MessageFlagsChanged"/> events will be emitted.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="uids"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="uids"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="uids"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open in read-write mode.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// The <see cref="MailFolder"/> does not support mod-sequences.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract UniqueId[] SetFlags (UniqueId[] uids, ulong modseq, MessageFlags flags, bool silent, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously set the flags of the specified messages only if their mod-sequence value is less than the specified value.
		/// </summary>
		/// <remarks>
		/// Asynchronously sets the flags of the specified messages only if their mod-sequence value is less than the specified value.
		/// </remarks>
		/// <returns>The unique IDs of the messages that were not updated.</returns>
		/// <param name="uids">The UIDs of the messages.</param>
		/// <param name="modseq">The mod-sequence value.</param>
		/// <param name="flags">The message flags to set.</param>
		/// <param name="silent">If set to <c>true</c>, no <see cref="MessageFlagsChanged"/> events will be emitted.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="uids"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="uids"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="uids"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open in read-write mode.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// The <see cref="MailFolder"/> does not support mod-sequences.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<UniqueId[]> SetFlagsAsync (UniqueId[] uids, ulong modseq, MessageFlags flags, bool silent, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (uids == null)
				throw new ArgumentNullException ("uids");

			if (uids.Length == 0)
				throw new ArgumentException ("No uids were specified.", "uids");

			return Task.Factory.StartNew (() => {
				return SetFlags (uids, modseq, flags, silent, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Add a set of flags to the specified messages.
		/// </summary>
		/// <remarks>
		/// Adds a set of flags to the specified messages.
		/// </remarks>
		/// <param name="indexes">The indexes of the messages.</param>
		/// <param name="flags">The message flags to add.</param>
		/// <param name="silent">If set to <c>true</c>, no <see cref="MessageFlagsChanged"/> events will be emitted.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="indexes"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="indexes"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="indexes"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open in read-write mode.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract void AddFlags (int[] indexes, MessageFlags flags, bool silent, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously add a set of flags to the specified messages.
		/// </summary>
		/// <remarks>
		/// Asynchronously adds a set of flags to the specified messages.
		/// </remarks>
		/// <param name="indexes">The indexes of the messages.</param>
		/// <param name="flags">The message flags to add.</param>
		/// <param name="silent">If set to <c>true</c>, no <see cref="MessageFlagsChanged"/> events will be emitted.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="indexes"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="indexes"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="indexes"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open in read-write mode.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task AddFlagsAsync (int[] indexes, MessageFlags flags, bool silent, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (indexes == null)
				throw new ArgumentNullException ("indexes");

			return Task.Factory.StartNew (() => {
				AddFlags (indexes, flags, silent, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Remove a set of flags from the specified messages.
		/// </summary>
		/// <remarks>
		/// Removes a set of flags from the specified messages.
		/// </remarks>
		/// <param name="indexes">The indexes of the messages.</param>
		/// <param name="flags">The message flags to remove.</param>
		/// <param name="silent">If set to <c>true</c>, no <see cref="MessageFlagsChanged"/> events will be emitted.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="indexes"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="indexes"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="indexes"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open in read-write mode.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract void RemoveFlags (int[] indexes, MessageFlags flags, bool silent, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously remove a set of flags from the specified messages.
		/// </summary>
		/// <remarks>
		/// Asynchronously removes a set of flags from the specified messages.
		/// </remarks>
		/// <param name="indexes">The indexes of the messages.</param>
		/// <param name="flags">The message flags to remove.</param>
		/// <param name="silent">If set to <c>true</c>, no <see cref="MessageFlagsChanged"/> events will be emitted.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="indexes"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="indexes"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="indexes"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open in read-write mode.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task RemoveFlagsAsync (int[] indexes, MessageFlags flags, bool silent, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (indexes == null)
				throw new ArgumentNullException ("indexes");

			return Task.Factory.StartNew (() => {
				RemoveFlags (indexes, flags, silent, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Set the flags of the specified messages.
		/// </summary>
		/// <remarks>
		/// Sets the flags of the specified messages.
		/// </remarks>
		/// <param name="indexes">The indexes of the messages.</param>
		/// <param name="flags">The message flags to set.</param>
		/// <param name="silent">If set to <c>true</c>, no <see cref="MessageFlagsChanged"/> events will be emitted.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="indexes"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="indexes"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="indexes"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open in read-write mode.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract void SetFlags (int[] indexes, MessageFlags flags, bool silent, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously set the flags of the specified messages.
		/// </summary>
		/// <remarks>
		/// Asynchronously sets the flags of the specified messages.
		/// </remarks>
		/// <param name="indexes">The indexes of the messages.</param>
		/// <param name="flags">The message flags to set.</param>
		/// <param name="silent">If set to <c>true</c>, no <see cref="MessageFlagsChanged"/> events will be emitted.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="indexes"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="indexes"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="indexes"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open in read-write mode.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task SetFlagsAsync (int[] indexes, MessageFlags flags, bool silent, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (indexes == null)
				throw new ArgumentNullException ("indexes");

			return Task.Factory.StartNew (() => {
				SetFlags (indexes, flags, silent, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Add a set of flags to the specified messages only if their mod-sequence value is less than the specified value.
		/// </summary>
		/// <remarks>
		/// Adds a set of flags to the specified messages only if their mod-sequence value is less than the specified value.
		/// </remarks>
		/// <returns>The indexes of the messages that were not updated.</returns>
		/// <param name="indexes">The indexes of the messages.</param>
		/// <param name="modseq">The mod-sequence value.</param>
		/// <param name="flags">The message flags to add.</param>
		/// <param name="silent">If set to <c>true</c>, no <see cref="MessageFlagsChanged"/> events will be emitted.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="indexes"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="indexes"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="indexes"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open in read-write mode.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// The <see cref="MailFolder"/> does not support mod-sequences.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract int[] AddFlags (int[] indexes, ulong modseq, MessageFlags flags, bool silent, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously add a set of flags to the specified messages only if their mod-sequence value is less than the specified value.
		/// </summary>
		/// <remarks>
		/// Asynchronously adds a set of flags to the specified messages only if their mod-sequence value is less than the specified value.
		/// </remarks>
		/// <returns>The indexes of the messages that were not updated.</returns>
		/// <param name="indexes">The indexes of the messages.</param>
		/// <param name="modseq">The mod-sequence value.</param>
		/// <param name="flags">The message flags to add.</param>
		/// <param name="silent">If set to <c>true</c>, no <see cref="MessageFlagsChanged"/> events will be emitted.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="indexes"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="indexes"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="indexes"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open in read-write mode.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// The <see cref="MailFolder"/> does not support mod-sequences.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<int[]> AddFlagsAsync (int[] indexes, ulong modseq, MessageFlags flags, bool silent, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (indexes == null)
				throw new ArgumentNullException ("indexes");

			return Task.Factory.StartNew (() => {
				return AddFlags (indexes, modseq, flags, silent, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Remove a set of flags from the specified messages only if their mod-sequence value is less than the specified value.
		/// </summary>
		/// <remarks>
		/// Removes a set of flags from the specified messages only if their mod-sequence value is less than the specified value.
		/// </remarks>
		/// <returns>The indexes of the messages that were not updated.</returns>
		/// <param name="indexes">The indexes of the messages.</param>
		/// <param name="modseq">The mod-sequence value.</param>
		/// <param name="flags">The message flags to remove.</param>
		/// <param name="silent">If set to <c>true</c>, no <see cref="MessageFlagsChanged"/> events will be emitted.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="indexes"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="indexes"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="indexes"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open in read-write mode.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// The <see cref="MailFolder"/> does not support mod-sequences.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract int[] RemoveFlags (int[] indexes, ulong modseq, MessageFlags flags, bool silent, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously remove a set of flags from the specified messages only if their mod-sequence value is less than the specified value.
		/// </summary>
		/// <remarks>
		/// Asynchronously removes a set of flags from the specified messages only if their mod-sequence value is less than the specified value.
		/// </remarks>
		/// <returns>The indexes of the messages that were not updated.</returns>
		/// <param name="indexes">The indexes of the messages.</param>
		/// <param name="modseq">The mod-sequence value.</param>
		/// <param name="flags">The message flags to remove.</param>
		/// <param name="silent">If set to <c>true</c>, no <see cref="MessageFlagsChanged"/> events will be emitted.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="indexes"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="indexes"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="indexes"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open in read-write mode.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// The <see cref="MailFolder"/> does not support mod-sequences.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<int[]> RemoveFlagsAsync (int[] indexes, ulong modseq, MessageFlags flags, bool silent, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (indexes == null)
				throw new ArgumentNullException ("indexes");

			return Task.Factory.StartNew (() => {
				return RemoveFlags (indexes, modseq, flags, silent, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Set the flags of the specified messages only if their mod-sequence value is less than the specified value.
		/// </summary>
		/// <remarks>
		/// Sets the flags of the specified messages only if their mod-sequence value is less than the specified value.
		/// </remarks>
		/// <returns>The indexes of the messages that were not updated.</returns>
		/// <param name="indexes">The indexes of the messages.</param>
		/// <param name="modseq">The mod-sequence value.</param>
		/// <param name="flags">The message flags to set.</param>
		/// <param name="silent">If set to <c>true</c>, no <see cref="MessageFlagsChanged"/> events will be emitted.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="indexes"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="indexes"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="indexes"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open in read-write mode.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// The <see cref="MailFolder"/> does not support mod-sequences.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract int[] SetFlags (int[] indexes, ulong modseq, MessageFlags flags, bool silent, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously set the flags of the specified messages only if their mod-sequence value is less than the specified value.
		/// </summary>
		/// <remarks>
		/// Asynchronously sets the flags of the specified messages only if their mod-sequence value is less than the specified value.
		/// </remarks>
		/// <returns>The indexes of the messages that were not updated.</returns>
		/// <param name="indexes">The indexes of the messages.</param>
		/// <param name="modseq">The mod-sequence value.</param>
		/// <param name="flags">The message flags to set.</param>
		/// <param name="silent">If set to <c>true</c>, no <see cref="MessageFlagsChanged"/> events will be emitted.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="indexes"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="indexes"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="indexes"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open in read-write mode.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// The <see cref="MailFolder"/> does not support mod-sequences.
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<int[]> SetFlagsAsync (int[] indexes, ulong modseq, MessageFlags flags, bool silent, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (indexes == null)
				throw new ArgumentNullException ("indexes");

			return Task.Factory.StartNew (() => {
				return SetFlags (indexes, modseq, flags, silent, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Search the folder for messages matching the specified query.
		/// </summary>
		/// <remarks>
		/// The returned array of unique identifiers can be used with <see cref="IMailFolder.GetMessage(UniqueId,CancellationToken)"/>.
		/// </remarks>
		/// <returns>An array of matching UIDs.</returns>
		/// <param name="query">The search query.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="query"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// One or more search terms in the <paramref name="query"/> are not supported by the mail store.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract UniqueId[] Search (SearchQuery query, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously search the folder for messages matching the specified query.
		/// </summary>
		/// <remarks>
		/// The returned array of unique identifiers can be used with <see cref="IMailFolder.GetMessage(UniqueId,CancellationToken)"/>.
		/// </remarks>
		/// <returns>An array of matching UIDs.</returns>
		/// <param name="query">The search query.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="query"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// One or more search terms in the <paramref name="query"/> are not supported by the mail store.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<UniqueId[]> SearchAsync (SearchQuery query, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (query == null)
				throw new ArgumentNullException ("query");

			return Task.Factory.StartNew (() => {
				return Search (query, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Search the folder for messages matching the specified query,
		/// returning them in the preferred sort order.
		/// </summary>
		/// <remarks>
		/// The returned array of unique identifiers will be sorted in the preferred order and
		/// can be used with <see cref="IMailFolder.GetMessage(UniqueId,CancellationToken)"/>.
		/// </remarks>
		/// <returns>An array of matching UIDs in the specified sort order.</returns>
		/// <param name="query">The search query.</param>
		/// <param name="orderBy">The sort order.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="query"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="orderBy"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="orderBy"/> is empty.
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// <para>One or more search terms in the <paramref name="query"/> are not supported by the mail store.</para>
		/// <para>-or-</para>
		/// <para>The server does not support the SORT extension.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract UniqueId[] Search (SearchQuery query, OrderBy[] orderBy, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously search the folder for messages matching the specified query,
		/// returning them in the preferred sort order.
		/// </summary>
		/// <remarks>
		/// The returned array of unique identifiers will be sorted in the preferred order and
		/// can be used with <see cref="IMailFolder.GetMessage(UniqueId,CancellationToken)"/>.
		/// </remarks>
		/// <returns>An array of matching UIDs in the specified sort order.</returns>
		/// <param name="query">The search query.</param>
		/// <param name="orderBy">The sort order.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="query"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="orderBy"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <paramref name="orderBy"/> is empty.
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// <para>One or more search terms in the <paramref name="query"/> are not supported by the mail store.</para>
		/// <para>-or-</para>
		/// <para>The server does not support the SORT extension.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<UniqueId[]> SearchAsync (SearchQuery query, OrderBy[] orderBy, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (query == null)
				throw new ArgumentNullException ("query");

			if (orderBy == null)
				throw new ArgumentNullException ("orderBy");

			if (orderBy.Length == 0)
				throw new ArgumentException ("No sort order provided.", "orderBy");

			return Task.Factory.StartNew (() => {
				return Search (query, orderBy, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Search the subset of UIDs in the folder for messages matching the specified query.
		/// </summary>
		/// <remarks>
		/// The returned array of unique identifiers can be used with <see cref="IMailFolder.GetMessage(UniqueId,CancellationToken)"/>.
		/// </remarks>
		/// <returns>An array of matching UIDs in the specified sort order.</returns>
		/// <param name="uids">The subset of UIDs</param>
		/// <param name="query">The search query.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="uids"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="query"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="uids"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="uids"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// One or more search terms in the <paramref name="query"/> are not supported by the mail store.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract UniqueId[] Search (UniqueId[] uids, SearchQuery query, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously search the subset of UIDs in the folder for messages matching the specified query.
		/// </summary>
		/// <remarks>
		/// The returned array of unique identifiers can be used with <see cref="IMailFolder.GetMessage(UniqueId,CancellationToken)"/>.
		/// </remarks>
		/// <returns>An array of matching UIDs in the specified sort order.</returns>
		/// <param name="uids">The subset of UIDs</param>
		/// <param name="query">The search query.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="uids"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="query"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="uids"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="uids"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// One or more search terms in the <paramref name="query"/> are not supported by the mail store.
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<UniqueId[]> SearchAsync (UniqueId[] uids, SearchQuery query, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (uids == null)
				throw new ArgumentNullException ("uids");

			if (uids.Length == 0)
				throw new ArgumentException ("No uids were specified.", "uids");

			if (query == null)
				throw new ArgumentNullException ("query");

			return Task.Factory.StartNew (() => {
				return Search (uids, query, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Search the subset of UIDs in the folder for messages matching the specified query,
		/// returning them in the preferred sort order.
		/// </summary>
		/// <remarks>
		/// The returned array of unique identifiers will be sorted in the preferred order and
		/// can be used with <see cref="IMailFolder.GetMessage(UniqueId,CancellationToken)"/>.
		/// </remarks>
		/// <returns>An array of matching UIDs.</returns>
		/// <param name="uids">The subset of UIDs</param>
		/// <param name="query">The search query.</param>
		/// <param name="orderBy">The sort order.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="uids"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="query"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="orderBy"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="uids"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="uids"/> is invalid.</para>
		/// <para>-or-</para>
		/// <para><paramref name="orderBy"/> is empty.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// <para>One or more search terms in the <paramref name="query"/> are not supported by the mail store.</para>
		/// <para>-or-</para>
		/// <para>The server does not support the SORT extension.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract UniqueId[] Search (UniqueId[] uids, SearchQuery query, OrderBy[] orderBy, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously search the subset of UIDs in the folder for messages matching the specified query,
		/// returning them in the preferred sort order.
		/// </summary>
		/// <remarks>
		/// The returned array of unique identifiers will be sorted in the preferred order and
		/// can be used with <see cref="IMailFolder.GetMessage(UniqueId,CancellationToken)"/>.
		/// </remarks>
		/// <returns>An array of matching UIDs.</returns>
		/// <param name="uids">The subset of UIDs</param>
		/// <param name="query">The search query.</param>
		/// <param name="orderBy">The sort order.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="uids"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="query"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="orderBy"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="uids"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="uids"/> is invalid.</para>
		/// <para>-or-</para>
		/// <para><paramref name="orderBy"/> is empty.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// <para>One or more search terms in the <paramref name="query"/> are not supported by the mail store.</para>
		/// <para>-or-</para>
		/// <para>The server does not support the SORT extension.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<UniqueId[]> SearchAsync (UniqueId[] uids, SearchQuery query, OrderBy[] orderBy, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (uids == null)
				throw new ArgumentNullException ("uids");

			if (uids.Length == 0)
				throw new ArgumentException ("No uids were specified.", "uids");

			if (query == null)
				throw new ArgumentNullException ("query");

			if (orderBy == null)
				throw new ArgumentNullException ("orderBy");

			if (orderBy.Length == 0)
				throw new ArgumentException ("No sort order provided.", "orderBy");

			return Task.Factory.StartNew (() => {
				return Search (uids, query, orderBy, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Thread the messages in the folder that match the search query using the specified threading algorithm.
		/// </summary>
		/// <remarks>
		/// The <see cref="MessageThread.UniqueId"/> can be used with <see cref="IMailFolder.GetMessage(UniqueId,CancellationToken)"/>.
		/// </remarks>
		/// <returns>An array of message threads.</returns>
		/// <param name="algorithm">The threading algorithm to use.</param>
		/// <param name="query">The search query.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="algorithm"/> is not supported.
		/// </exception>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="query"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// <para>One or more search terms in the <paramref name="query"/> are not supported by the mail store.</para>
		/// <para>-or-</para>
		/// <para>The server does not support the THREAD extension.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract MessageThread[] Thread (ThreadingAlgorithm algorithm, SearchQuery query, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously thread the messages in the folder that match the search query using the specified threading algorithm.
		/// </summary>
		/// <remarks>
		/// The <see cref="MessageThread.UniqueId"/> can be used with <see cref="IMailFolder.GetMessage(UniqueId,CancellationToken)"/>.
		/// </remarks>
		/// <returns>An array of message threads.</returns>
		/// <param name="algorithm">The threading algorithm to use.</param>
		/// <param name="query">The search query.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="algorithm"/> is not supported.
		/// </exception>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="query"/> is <c>null</c>.
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// <para>One or more search terms in the <paramref name="query"/> are not supported by the mail store.</para>
		/// <para>-or-</para>
		/// <para>The server does not support the THREAD extension.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<MessageThread[]> ThreadAsync (ThreadingAlgorithm algorithm, SearchQuery query, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (query == null)
				throw new ArgumentNullException ("query");

			return Task.Factory.StartNew (() => {
				return Thread (algorithm, query, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Thread the messages in the folder that match the search query using the specified threading algorithm.
		/// </summary>
		/// <remarks>
		/// The <see cref="MessageThread.UniqueId"/> can be used with <see cref="IMailFolder.GetMessage(UniqueId,CancellationToken)"/>.
		/// </remarks>
		/// <returns>An array of message threads.</returns>
		/// <param name="uids">The subset of UIDs</param>
		/// <param name="algorithm">The threading algorithm to use.</param>
		/// <param name="query">The search query.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="algorithm"/> is not supported.
		/// </exception>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="uids"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="query"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="uids"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="uids"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// <para>One or more search terms in the <paramref name="query"/> are not supported by the mail store.</para>
		/// <para>-or-</para>
		/// <para>The server does not support the THREAD extension.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public abstract MessageThread[] Thread (UniqueId[] uids, ThreadingAlgorithm algorithm, SearchQuery query, CancellationToken cancellationToken = default (CancellationToken));

		/// <summary>
		/// Asynchronously thread the messages in the folder that match the search query using the specified threading algorithm.
		/// </summary>
		/// <remarks>
		/// The <see cref="MessageThread.UniqueId"/> can be used with <see cref="IMailFolder.GetMessage(UniqueId,CancellationToken)"/>.
		/// </remarks>
		/// <returns>An array of message threads.</returns>
		/// <param name="uids">The subset of UIDs</param>
		/// <param name="algorithm">The threading algorithm to use.</param>
		/// <param name="query">The search query.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// <paramref name="algorithm"/> is not supported.
		/// </exception>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="uids"/> is <c>null</c>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="query"/> is <c>null</c>.</para>
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// <para><paramref name="uids"/> is empty.</para>
		/// <para>-or-</para>
		/// <para>One or more of the <paramref name="uids"/> is invalid.</para>
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// <para>One or more search terms in the <paramref name="query"/> are not supported by the mail store.</para>
		/// <para>-or-</para>
		/// <para>The server does not support the THREAD extension.</para>
		/// </exception>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		/// <exception cref="System.OperationCanceledException">
		/// The operation was canceled via the cancellation token.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An I/O error occurred.
		/// </exception>
		/// <exception cref="ProtocolException">
		/// The server's response contained unexpected tokens.
		/// </exception>
		/// <exception cref="CommandException">
		/// The command failed.
		/// </exception>
		public virtual Task<MessageThread[]> ThreadAsync (UniqueId[] uids, ThreadingAlgorithm algorithm, SearchQuery query, CancellationToken cancellationToken = default (CancellationToken))
		{
			if (uids == null)
				throw new ArgumentNullException ("uids");

			if (uids.Length == 0)
				throw new ArgumentException ("No uids were specified.", "uids");

			if (query == null)
				throw new ArgumentNullException ("query");

			return Task.Factory.StartNew (() => {
				return Thread (uids, algorithm, query, cancellationToken);
			}, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
		}

		/// <summary>
		/// Occurs when the folder is deleted.
		/// </summary>
		/// <remarks>
		/// The <see cref="Deleted"/> event is emitted when the folder is deleted.
		/// </remarks>
		public event EventHandler<EventArgs> Deleted;

		/// <summary>
		/// Raise the deleted event.
		/// </summary>
		/// <remarks>
		/// Raises the deleted event.
		/// </remarks>
		protected void OnDeleted ()
		{
			var handler = Deleted;

			if (handler != null)
				handler (this, EventArgs.Empty);
		}

		/// <summary>
		/// Occurs when the folder is renamed.
		/// </summary>
		/// <remarks>
		/// The <see cref="Renamed"/> event is emitted when the folder is renamed.
		/// </remarks>
		public event EventHandler<FolderRenamedEventArgs> Renamed;

		/// <summary>
		/// Raise the renamed event.
		/// </summary>
		/// <param name="oldName">The old name of the folder.</param>
		/// <param name="newName">The new name of the folder.</param>
		protected void OnRenamed (string oldName, string newName)
		{
			var handler = Renamed;

			if (handler != null)
				handler (this, new FolderRenamedEventArgs (oldName, newName));
		}

		/// <summary>
		/// Occurs when the folder is subscribed.
		/// </summary>
		/// <remarks>
		/// The <see cref="Subscribed"/> event is emitted when the folder is subscribed.
		/// </remarks>
		public event EventHandler<EventArgs> Subscribed;

		/// <summary>
		/// Raise the subscribed event.
		/// </summary>
		/// <remarks>
		/// Raises the subscribed event.
		/// </remarks>
		protected void OnSubscribed ()
		{
			var handler = Subscribed;

			if (handler != null)
				handler (this, EventArgs.Empty);
		}

		/// <summary>
		/// Occurs when the folder is unsubscribed.
		/// </summary>
		/// <remarks>
		/// The <see cref="Unsubscribed"/> event is emitted when the folder is unsubscribed.
		/// </remarks>
		public event EventHandler<EventArgs> Unsubscribed;

		/// <summary>
		/// Raise the unsubscribed event.
		/// </summary>
		/// <remarks>
		/// Raises the unsubscribed event.
		/// </remarks>
		protected void OnUnsubscribed ()
		{
			var handler = Unsubscribed;

			if (handler != null)
				handler (this, EventArgs.Empty);
		}

		/// <summary>
		/// Occurs when a message is expunged from the folder.
		/// </summary>
		/// <remarks>
		/// The <see cref="MessageExpunged"/> event is emitted when a message is expunged from the folder.
		/// </remarks>
		public event EventHandler<MessageEventArgs> MessageExpunged;

		/// <summary>
		/// Raise the message expunged event.
		/// </summary>
		/// <remarks>
		/// Raises the message expunged event.
		/// </remarks>
		/// <param name="args">The message expunged event args.</param>
		protected void OnMessageExpunged (MessageEventArgs args)
		{
			var handler = MessageExpunged;

			if (handler != null)
				handler (this, args);
		}

		/// <summary>
		/// Occurs when a message vanishes from the folder.
		/// </summary>
		/// <remarks>
		/// The <see cref="MessagesVanished"/> event is emitted when messages vanish from the folder.
		/// </remarks>
		public event EventHandler<MessagesVanishedEventArgs> MessagesVanished;

		/// <summary>
		/// Raise the messages vanished event.
		/// </summary>
		/// <remarks>
		/// Raises the messages vanished event.
		/// </remarks>
		/// <param name="args">The messages vanished event args.</param>
		protected void OnMessagesVanished (MessagesVanishedEventArgs args)
		{
			var handler = MessagesVanished;

			if (handler != null)
				handler (this, args);
		}

		/// <summary>
		/// Occurs when flags changed on a message.
		/// </summary>
		/// <remarks>
		/// The <see cref="MessageFlagsChanged"/> event is emitted when the flags for a message are changed.
		/// </remarks>
		public event EventHandler<MessageFlagsChangedEventArgs> MessageFlagsChanged;

		/// <summary>
		/// Raise the flags changed event.
		/// </summary>
		/// <remarks>
		/// Raises the flags changed event.
		/// </remarks>
		/// <param name="args">The message flags changed event args.</param>
		protected void OnFlagsChanged (MessageFlagsChangedEventArgs args)
		{
			var handler = MessageFlagsChanged;

			if (handler != null)
				handler (this, args);
		}

		/// <summary>
		/// Occurs when the UID validity changes.
		/// </summary>
		/// <remarks>
		/// The <see cref="UidValidityChanged"/> event is emitted whenever the <see cref="UidValidity"/> value changes.
		/// </remarks>
		public event EventHandler<EventArgs> UidValidityChanged;

		/// <summary>
		/// Raise the uid validity changed event.
		/// </summary>
		/// <remarks>
		/// Raises the uid validity changed event.
		/// </remarks>
		protected void OnUidValidityChanged ()
		{
			var handler = UidValidityChanged;

			if (handler != null)
				handler (this, EventArgs.Empty);
		}

		/// <summary>
		/// Occurs when the message count changes.
		/// </summary>
		/// <remarks>
		/// The <see cref="CountChanged"/> event is emitted whenever the <see cref="Count"/> value changes.
		/// </remarks>
		public event EventHandler<EventArgs> CountChanged;

		/// <summary>
		/// Raise the count changed event.
		/// </summary>
		/// <remarks>
		/// Raises the count changed event.
		/// </remarks>
		protected void OnCountChanged ()
		{
			var handler = CountChanged;

			if (handler != null)
				handler (this, EventArgs.Empty);
		}

		/// <summary>
		/// Occurs when the recent message count changes.
		/// </summary>
		/// <remarks>
		/// The <see cref="RecentChanged"/> event is emitted whenever the <see cref="Recent"/> value changes.
		/// </remarks>
		public event EventHandler<EventArgs> RecentChanged;

		/// <summary>
		/// Raise the recent changed event.
		/// </summary>
		/// <remarks>
		/// Raises the recent changed event.
		/// </remarks>
		protected void OnRecentChanged ()
		{
			var handler = RecentChanged;

			if (handler != null)
				handler (this, EventArgs.Empty);
		}

		#region IEnumerable<MimeMessage> implementation

		/// <summary>
		/// Get an enumerator for the messages in the folder.
		/// </summary>
		/// <remarks>
		/// Gets an enumerator for the messages in the folder.
		/// </remarks>
		/// <returns>The enumerator.</returns>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		public abstract IEnumerator<MimeMessage> GetEnumerator ();

		/// <summary>
		/// Get an enumerator for the messages in the folder.
		/// </summary>
		/// <remarks>
		/// Gets an enumerator for the messages in the folder.
		/// </remarks>
		/// <returns>The enumerator.</returns>
		/// <exception cref="System.ObjectDisposedException">
		/// The <see cref="IMailStore"/> has been disposed.
		/// </exception>
		/// <exception cref="System.InvalidOperationException">
		/// <para>The <see cref="IMailStore"/> is not connected.</para>
		/// <para>-or-</para>
		/// <para>The <see cref="IMailStore"/> is not authenticated.</para>
		/// <para>-or-</para>
		/// <para>The folder is not currently open.</para>
		/// </exception>
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}

		#endregion

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="MailKit.MailFolder"/>.
		/// </summary>
		/// <remarks>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="MailKit.MailFolder"/>.
		/// </remarks>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="MailKit.MailFolder"/>.</returns>
		public override string ToString ()
		{
			return FullName;
		}
	}
}