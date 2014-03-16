using System;
using System.IO;

using PushSharp;
using PushSharp.Apple;
using PushSharp.Core;

namespace PNSTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");

			//Create our push services broker
			var push = new PushBroker();

			//Wire up the events for all the services that the broker registers
			push.OnNotificationSent += NotificationSent;
			push.OnChannelException += ChannelException;
			push.OnServiceException += ServiceException;
			push.OnNotificationFailed += NotificationFailed;
			push.OnDeviceSubscriptionExpired += DeviceSubscriptionExpired;
			push.OnDeviceSubscriptionChanged += DeviceSubscriptionChanged;
			push.OnChannelCreated += ChannelCreated;
			push.OnChannelDestroyed += ChannelDestroyed;

			//Registering the Apple Service and sending an iOS Notification
			string certPath = Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "../../Resources/Apns.Sandbox.p12");
			Console.WriteLine(string.Format("Certificate path: {0}", certPath));
			var appleCert = File.ReadAllBytes(certPath);

			var isProduction = false;

			//isProduction - используете ли вы development или ad-hoc/release сертификат
			push.RegisterAppleService(new ApplePushChannelSettings(isProduction, appleCert, "1"));


			push.QueueNotification(new AppleNotification()
				.ForDeviceToken("fa1c84bc057eb541f6dd99b1c084cd4c77fba9a0006859fbd9fbf1c2037047bd")
				.WithAlert("Hello World!")
				.WithBadge(0)
				.WithSound("sound.caf"));

			Console.WriteLine("Waiting for Queue to Finish...");

			//Stop and wait for the queues to drains
			push.StopAllServices();

			Console.WriteLine("Queue Finished, press return to exit...");
			Console.ReadLine();

		}

		static void DeviceSubscriptionChanged(object sender, string oldSubscriptionId, string newSubscriptionId, INotification notification)
		{
			//Currently this event will only ever happen for Android GCM
			Console.WriteLine("Device Registration Changed:  Old-> " + oldSubscriptionId + "  New-> " + newSubscriptionId + " -> " + notification);
		}

		static void NotificationSent(object sender, INotification notification)
		{
			Console.WriteLine("Sent: " + sender + " -> " + notification);
		}

		static void NotificationFailed(object sender, INotification notification, Exception notificationFailureException)
		{
			Console.WriteLine("Failure: " + sender + " -> " + notificationFailureException.Message + " -> " + notification);
		}

		static void ChannelException(object sender, IPushChannel channel, Exception exception)
		{
			Console.WriteLine("Channel Exception: " + sender + " -> " + exception);
		}

		static void ServiceException(object sender, Exception exception)
		{
			Console.WriteLine("Channel Exception: " + sender + " -> " + exception);
		}

		static void DeviceSubscriptionExpired(object sender, string expiredDeviceSubscriptionId, DateTime timestamp, INotification notification)
		{
			Console.WriteLine("Device Subscription Expired: " + sender + " -> " + expiredDeviceSubscriptionId);
		}

		static void ChannelDestroyed(object sender)
		{
			Console.WriteLine("Channel Destroyed for: " + sender);
		}

		static void ChannelCreated(object sender, IPushChannel pushChannel)
		{
			Console.WriteLine("Channel Created for: " + sender);
		}
	}
}
