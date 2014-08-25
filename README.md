Manufacturing.DevRunner
=======================

# Background

This project is part of a series of projects that are designed to serve as a reference implementation focusing on the unique needs of discrete and process manufacturing, which more weight on the discrete portion.

### This framework is...

* focused on the MES portion of discrete manufacturing.
* an end-to-end data pipeline capable of pulling data from existing systems (using adapters) and ultimately display that information and provide self-service business intelligence.
* decomposable - components are interface-based so that any portions can be used individually. Use as little or as much as you like.
extensible - because of the modular design approach, the framework can be extended limitlessly.
* open source - licensed under the Apache 2 license
* using Microsoft technologies such as Windows Azure and Windows 8, although non-Microsoft technologies will be used where appropriate.
* aligned with tomorrows manufacturing trends such as Industry 4.0.

### This framework is not...

* competing with other Microsoft efforts. It's meant to fill in gaps, not replace existing solutions in development.
* the best way to push data. It simply demonstrates one possible solution.
* the best way to store data. It simply demonstrates one possible solution.
* the best way to process data. It simply demonstrates one possible solution.
* embedded. This framework is a level above the embedded device ecosystem, but can use data generated or collected by devices.
* competing with partners already in this space. This is designed to help accelerate partner application development, and gives them opportunities to add their business value.

# Key Project List

[**Manufacturing.DevRunner**](https://github.com/ytechie/Manufacturing.DevRunner): Contains some of the common files to work with the various manufacturing project resources. It includes a solution file and a console application that makes it easy to run and develop.

[**Manufacturing.Framework**](https://github.com/ytechie/Manufacturing.Framework): Contains common object types and some utility functionality.

[**Manufacturing.DataCollector**](https://github.com/ytechie/Manufacturing.DataCollector): A service that can collect data from various sources on a schedule, as well as accept data through a self-hosted API. The data is stored in a local, persistent cache.

[**Manufacturing.DataPusher**](https://github.com/ytechie/Manufacturing.DataPusher): A service to move data from the local, persistent cache to a cloud provider.

[**Manufacturing.FacilityDataReceiver**](https://github.com/ytechie/Manufacturing.FacilityDataReceiver): An service that processes and dispatches data coming into the cloud.

[**Manufacturing.Azure**](https://github.com/ytechie/Manufacturing.Azure): A cloud service project for Azure deployment.

[**Manufacturing.WinApp**](https://github.com/ytechie/Manufacturing.WinApp): A Windows 8.1 demo application

[**Manufacturing.Api**](https://github.com/ytechie/Manufacturing.Api): An API for interacting with the portion of the framework running in the cloud.

[**Manufacturing.Orleans**](https://github.com/ytechie/Manufacturing.Orleans): An Orleans project for working with manufacturing sensors and other metadata.

# Installation

To get all of the manufacturing sample projects, clone all of the repositories, preferably into a dedicated directory.

	git clone https://github.com/ytechie/Manufacturing.Framework
	git clone https://github.com/ytechie/Manufacturing.DataCollector
	git clone https://github.com/ytechie/Manufacturing.DataPusher
	git clone https://github.com/ytechie/Manufacturing.FacilityDataReceiver
	git clone https://github.com/ytechie/Manufacturing.DevRunner
	git clone https://github.com/ytechie/Manufacturing.Azure
	git clone https://github.com/ytechie/Manufacturing.WinApp
	git clone https://github.com/ytechie/Manufacturing.Api
	git clone https://github.com/ytechie/Manufacturing.Orleans

Open PowerShell as an administrator and execute the following. Feel free to examine the script, but it's purpose is to set the policies for the self-hosted WebAPI as well as install the required Windows components.

Sample commands:

	Set-ExecutionPolicy Unrestricted
	cd Manufacturing.DevRunner\scripts
	./installDev.ps1

Once you have run this script, feel free to run `Set-ExecutionPolicy Restricted` to change your PowerShell policy back to the default.

### Configuration

After grabbing the source, you'll need to configure some parameters so that the pieces know how to talk to things like the Azure Service Bus. Look inside each project for a `Configuration` folder, and set the settings within the JSON files. These JSON files get loaded into classes at runtime using the [Convention Configuration](https://github.com/ytechie/ConventionConfig) library.

### Project Planning

Check out the [public Trello planning board](https://trello.com/b/CbdL95oD/manufacturing-framework).

### Contributors

#### Microsoft
Jason Young
Hanu Kommalapati
Mike Zawacki
Tony Guidici

#### Skyline Technologies
Kenny Young
Greg Levanhagen
Brandon Martinez
Chris Plate
Mike Tovbin
Steve Nelson
Paul Shepard
Samuel Lees

# License

Microsoft Developer Experience & Evangelism

Copyright (c) Microsoft Corporation. All rights reserved.

THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.

The example companies, organizations, products, domain names, e-mail addresses, logos, people, places, and events depicted herein are fictitious. No association with any real company, organization, product, domain name, email address, logo, person, places, or events is intended or should be inferred.
