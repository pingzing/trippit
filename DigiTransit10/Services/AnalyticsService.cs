using DigiTransit10.Services.SettingsServices;
using Microsoft.HockeyApp;
using System;
using System.Collections.Generic;

namespace DigiTransit10.Services
{
    public interface IAnalyticsService
    {
        void TrackDependency(string dependencyName, string command, DateTimeOffset startTime, TimeSpan duration, bool success);
        void TrackEvent(string eventName, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null);
        void TrackException(Exception ex, IDictionary<string, string> properties = null);
        void TrackMetric(string name, double value, IDictionary<string, string> properties);
        void TrackPageView(string name);
        void TrackTrace(string message);
        void TrackTrace(string message, SeverityLevel severityLevel);
        void TrackTrace(string message, IDictionary<string, string> properties);
        void TrackTrace(string message, SeverityLevel severityLevel, IDictionary<string, string> properties);
    }

    public class AnalyticsService : IAnalyticsService
    {
        private SettingsService _settings;
        private IHockeyClient _hockeyClient = HockeyClient.Current;

        public AnalyticsService(SettingsService settings)
        {
            _hockeyClient.Configure("c2a732e8165446bc81e0ea6087509c2b");
            _settings = settings;
        }

        public void TrackDependency(string dependencyName, string command, DateTimeOffset startTime, TimeSpan duration, bool success)
        {
            if (_settings.IsAnalyticsEnabled)
            {
                _hockeyClient.TrackDependency(dependencyName, command, startTime, duration, success);
            }
        }

        public void TrackEvent(string eventName, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
            if (_settings.IsAnalyticsEnabled)
            {
                _hockeyClient.TrackEvent(eventName, properties, metrics);
            }
        }

        public void TrackException(Exception ex, IDictionary<string, string> properties = null)
        {
            if (_settings.IsAnalyticsEnabled)
            {
                _hockeyClient.TrackException(ex, properties);
            }
        }

        public void TrackMetric(string name, double value, IDictionary<string, string> properties)
        {
            if (_settings.IsAnalyticsEnabled)
            {
                _hockeyClient.TrackMetric(name, value, properties);
            }
        }

        public void TrackPageView(string name)
        {
            if (_settings.IsAnalyticsEnabled)
            {
                _hockeyClient.TrackPageView(name);
            }
        }

        public void TrackTrace(string message)
        {
            if (_settings.IsAnalyticsEnabled)
            {
                _hockeyClient.TrackTrace(message);
            }
        }

        public void TrackTrace(string message, SeverityLevel severityLevel)
        {
            if (_settings.IsAnalyticsEnabled)
            {
                _hockeyClient.TrackTrace(message, severityLevel);
            }
        }

        public void TrackTrace(string message, IDictionary<string, string> properties)
        {
            if (_settings.IsAnalyticsEnabled)
            {
                _hockeyClient.TrackTrace(message, properties);
            }
        }

        public void TrackTrace(string message, SeverityLevel severityLevel, IDictionary<string, string> properties)
        {
            if (_settings.IsAnalyticsEnabled)
            {
                _hockeyClient.TrackTrace(message, severityLevel, properties);
            }
        }
    }
}
