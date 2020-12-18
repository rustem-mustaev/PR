﻿using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PRmetricWeb.Models
{
    public class UserList
    {
        private string TOKEN = "oo6ysqwey3fjknf6o52f6kcv2dnxpsvoyj3ufdv3n5pdzdv354va";
        private string URL = "https://dev.azure.com/prmetricstest/";
        private string PROJECT_NAME = "test";
        private string REPOS_NAME = "test";

        private GitHttpClient GitClient;
        private TeamHttpClient TeamClient;
        private WorkItemTrackingHttpClient WorkItemClient;

        public UserList(string url, string project, string repos, string token)
        {
            TOKEN = token;
            URL = url;
            PROJECT_NAME = project;
            REPOS_NAME = repos;
            SetConnect();
        }

        public List<User> GetUsers()
        {
            var teams = TeamClient.GetTeamsAsync(PROJECT_NAME).Result;
            var teamId = teams[0].Id.ToString();
            var teamMember = TeamClient
                .GetTeamMembersWithExtendedPropertiesAsync(PROJECT_NAME, teamId).Result;
            List<User> data = new List<User>();

            foreach (var member in teamMember)
            {
                var ID = member.Identity.Id;
                var name = member.Identity.DisplayName;
                var opened = ViewPullCount(PROJECT_NAME, ID, REPOS_NAME);
                var closed = ViewPullCount(PROJECT_NAME, ID, REPOS_NAME, true);
                var totalStoryPoints = GetTotalStoryPoint(ID);

                data.Add(new User()
                {
                    Id = ID,
                    Name = name,
                    Open= opened,
                    Closed = closed,
                    All = opened + closed,
                    TotalPoints = totalStoryPoints
                });
            }

            return data;
        }

        private void SetConnect()
        {
            var connection = new VssConnection(
                new Uri(URL),
                new VssBasicCredential(string.Empty, TOKEN)
                );
            GitClient = connection.GetClient<GitHttpClient>();
            TeamClient = connection.GetClient<TeamHttpClient>();
            WorkItemClient = connection.GetClient<WorkItemTrackingHttpClient>();
        }

        private int ViewPullCount(string TeamProjectName, string userId, string GitRepo, bool CompletedPRs = false)
        {
            var pullRequests = (CompletedPRs) ?
               GitClient.GetPullRequestsAsync(
                    TeamProjectName,
                    GitRepo,
                    new GitPullRequestSearchCriteria
                    {
                        Status = PullRequestStatus.Completed,
                        CreatorId = Guid.Parse(userId)
                    }
               ).Result :
               GitClient.GetPullRequestsAsync(
                    TeamProjectName,
                    GitRepo,
                    new GitPullRequestSearchCriteria
                    {
                        CreatorId = Guid.Parse(userId)
                    },
                    null
               ).Result;
            return pullRequests.Count;
        }

        public double GetTotalStoryPoint(string userId)
        {
            List<GitPullRequest> prOpen = GetPullRequests(PROJECT_NAME, userId, REPOS_NAME);
            List<GitPullRequest> prClosed = GetPullRequests(PROJECT_NAME, userId, REPOS_NAME, true);
            List<GitPullRequest> prList = prOpen.Union(prClosed).ToList();

            double count = 0;

            foreach (GitPullRequest pr in prList)
            {
                var workItems = GetWorkItemsList(pr.PullRequestId);

                foreach (ResourceRef wit in workItems)
                {
                    var workItem = GetWorkItem(Convert.ToInt32(wit.Id));
                    if (workItem.Fields.TryGetValue(
                        "Microsoft.VSTS.Scheduling.StoryPoints",
                        out object storyPoint))
                    {
                        count += (double)storyPoint;
                    }
                }
            }

            return count;
        }

        public List<ResourceRef> GetWorkItemsList(int prId)
        {
            return GetWorkItems(PROJECT_NAME, REPOS_NAME, prId);
        }

        public List<ResourceRef> GetWorkItems(string projectName, string gitRepo, int prId)
        {
            return GitClient.GetPullRequestWorkItemRefsAsync(projectName, gitRepo, prId).Result;
        }

        public WorkItem GetWorkItem(int id)
        {
            return WorkItemClient.GetWorkItemAsync(PROJECT_NAME, id).Result;
        }

        private List<GitPullRequest> GetPullRequests(string TeamProjectName, string userId, string GitRepo, bool CompletedPRs = false)
        {
            var pullRequests = (CompletedPRs) ?
               GitClient.GetPullRequestsAsync(
                   TeamProjectName,
                   GitRepo,
                   new GitPullRequestSearchCriteria
                   {
                       Status = PullRequestStatus.Completed,
                       CreatorId = Guid.Parse(userId)
                   }).Result :
               GitClient.GetPullRequestsAsync(
                   TeamProjectName,
                   GitRepo,
                   new GitPullRequestSearchCriteria { CreatorId = Guid.Parse(userId) },
                   null
                   ).Result;
            return pullRequests;
        }
    }
}
