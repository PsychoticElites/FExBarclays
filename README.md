[![N|Solid](http://i.imgur.com/JcUlhcw.png)](https://github.com/PsychoticElites/FExBarclays)
# FExBarclays


## 1.	Introduction
This application aims to complete the challenges 1, 2 and 4 of the UI/UX Theme of #UNITEDBYHCL Hackathon, 2017. This application is in prototype phase, but works like any full scale deployed application. With this Formula E application users can Sign Up, Log In, view upcoming races, Driver’s information, watch Live streams of the ongoing events and find any nearby places to explore by using the integrated Google Maps. Furthermore, the users can also locate the nearest Barclays Bank ATM or Branch using voice enabled commands specially designed for visually impaired people.

The application also implements Street-View of the race-venue that helps race-goers find nearby Restrooms, Grandstands, Cafeterias or any other basic necessities. As mentioned in our “Idea Phase”, we have successfully implemented all the features and have managed to merge it in an “all-in-one” android application.

Our prototype completes the “Trackside Wayfinding” and “Second Screen Viewing at Trackside”, for Formula E. And, it also completes “iSight”, for Barclays. Please check the further sections to know more about these features in detail.
Whole android application is built on .Net and Xamarin Framework, and hence, can also be ported across platforms like - iOS and Windows.


## 2.	Objectives
  - User Friendly Interface
o	A more engaging and easy-to-use Graphical User Interface for all types of users, ranging from developers to non-technical users.
o	Live feed of overall track, driver's view and more.

 - Trackside Wayfinding
o	Application provides clear and detailed annotated Google maps along with Street-View of the stadium facilities for easier navigation of users present at stand on the day of race.
o	It enables race-goers to find all the basic necessities like Restrooms, Cafeterias or Grandstands and easily navigate to them using Google Street Navigation.

- Unified Application
o	Application provides both, user-friendly management and a near real-time view of the ongoing races.
o	Hub for all Formula - E activities such as "Upcoming Races", "Driver's Profile", "Track Overview", and constantly updated Formula - E News for Hardcore Fans.

- User customized Live Feed
o	Users can choose Live-Feeds as their preferences.
o	User can choose between 4 different camera modes (specified by circuit's camera settings), like driver camera, track overview, TV camera and aerial feed.

- Faster and More Efficient
o	Application is fast to boot-up and load all required data requested by user.
o	Application uses Cache to pre-fetch the resources.
o	Low RAM usage ensures efficient working of application.
o	Smoother animations and transitions during operation of application.


## 3.	Challenges
 - ## Challenge 01 (Trackside Wayfinding) 
     - As mentioned in the introduction section, we have successfully implemented the Trackside wayfinding for Formula E circuits. We have leveraged Google Maps and have incorporated it into our application to show the race-goers the important information about the area around them.
Whether you need a restaurant to satisfy your hunger, or you need to find the closest restroom, or maybe you wanted to check out the Grandstand or Formula E headquarter, we’ve got you covered.
Users can simply tap on the annotated places to find information regarding the same. We’ve categorized everything, so you don’t have to go through a large list of text, just to find a restaurant. Just tap on the intended category, and the application will show you all the closest Restrooms/Restaurants/Grandstands etc.

     - ur trackside wayfinding doesn’t stop at this, just showing the annotated map won’t help the user much. Therefore, you can also choose to get directions to navigate from your current GPS location to your intended destination, since our application is backed by the powerful Google Maps, you get detailed information about the road conditions to avoid traffic or find the shortest routes.

- ## Challenge 02 (Second Screen Viewing at Trackside)
     - To enhance User experience, we decided to merge the Challenge 2, i.e. Second Screen Viewing At Trackside into Challenge 1. 
Users have a choice of total 4 video streams to choose from. All the streams provide different camera angles and the users can choose whatever view they want to watch the stream from.

     - We are utilising Android’s powerful Video-Player for live streaming Formula E races. The playback depends upon the user’s handset. The video player can play AAC, HE-AACv1, HE-AACv2, FLAC, mp3, Opus, Wave and many more audio formats. And, as for the video playback, the player supports H.263, H.264 Baseline Profile, H.264 Main Profile (Android 6.0 +), HEVC (H.265) streams. Video resolutions can range from SD (176 x 144 px) (Low Quality) to HD (1280 x 720 px) (High Definition), all this up to 30 fps. 

- ## Challenge 04 (iSight - For Barclays)
     - As we have mentioned in our Project Proposal in the “Idea Phase - I”, we wanted to implement a feature for visually impaired people to be able to find the nearest Barclays Bank Branch or ATM. 
We have successfully made a total hands-free environment. Users can interact with the application using voice commands and the application will respond to user’s feedback accordingly. But, if for some reason, the application doesn’t understand what the user said, it’ll ask again until it gets it right. We’ve utilized the inbuilt API of android for Speech-To-Text and Text-To-Speech.

     - Application is built to be as detailed as it can be. It will provide user with all the relevant information about Bank and ATM. Users won’t have to lift their fingers to navigate within the application, they can just interact with it via voice commands, as mentioned earlier. 
Application recites nearest ATM’s location and its distance from user’s current location. While, for a bank branch, it recited its address, distance, contact number and tells whether the bank branch is currently opened or closed.

     - To enhance User experience, if any ATM near the user (5000 meters) or the bank branch is currently opened, then it’ll automatically open the default Maps application to find the shortest route to it and highlight it on the map. User just need to start their journey. 

## 4.	Features
This prototype application contains some of the Industry-standard features such as 
- Material Design.
- Faster Bootup.
- Latest race statistics.
- 4 Live stream views to choose from.
- Trackside navigation to nearby Restrooms, Cafes, Grandstands and Formula E HQ.
- Details about previous and upcoming race events, based on countries.
- Details about your favourite driver.
- Easy to use navigation around the race circuits.
- Uses APIs to get the content in real time.
- Ticket Booking.
- Social Media Interaction

## 5.	Extra Features
We have also tried to incorporate few extra features in the application for the users. 
- Users can buy tickets.
- Check out upcoming race events.
- Race schedules.
- Detailed information on Formula E drivers etc.
Driver information and race schedules are fetched on the fly via APIs we’ve made for test purposes. So, the information is updated at one end and the change is reflected in real-time on the devices everywhere else.

## 6.	Future Scope/ To Do
If selected for the 3rd round, we would like to optimize this application further and add some more features in this application, like 
- VR ready streams.
- Add an upcoming event in user's schedule.
- Login/ Sign Up via various social platforms.
- Simplify the problems faced by a race-goer.
- More features to mesmerize the experience of Users.

## 7.	Technology Stack Used
- ### Front End Development

    - ##### User Interface design:
       - Android XML (.axml)

    - ##### Other Softwares: 
       - GIMP

- ### Back End Development

    - ##### Main programming language:
       - C# - to develop base functionalities of proposed application.

    - ##### Basic scripting languages:
       - PHP – for Parsing back-end
       - Core Java – to Integrate and use Google Maps API
       - Visual Scripts for Unreal Engine 4 – to develop Virtual Reality (VR) mode of the application.
       - Unreal Engine 4 specific C++ - to optimise native code for Virtual Reality (VR) application.

-    ### Frameworks

    - Xamarin Framework– to develop native portable Android Application in C#
    - .NET Framework– to use Microsoft’s Visual Studio and C# basic packages.

-    ### Packages

    - C# Basic Packages– provided by Microsoft.
    - Newtonsoft – To parse JSON data.
    - Google Accessibility Packages – to utilize the microphone and other resources.
    - Google Play Services – Packages provided by Google for Android Development.

-    ### Application Programmable Interfaces (APIs)

    - Google Maps – to Implement Wayfinding Services.
    - Live Video Feeds – provided by Formula E, to Implement 4 Screen viewing in the proposed application.

-    ### VR Engine

    - Unreal Engine 4– to develop and implement Virtual Reality (VR) mode of the proposed application.

## 8.	Team’s Area of Expertise
- ### Front End Developer
    - **Ankit Passi** **(** UI/UX Designer **)**
•	Photoshop
•	GIMP
•	Illustrator
•	C#
•	C++
•	Xamarin Framework
•	VR Developer
•	well-versed with Unreal Engine Visual Scripting.


- ### Back End Developers
    - **Dhruv Kanojia** **(** Lead Developer **)**
•	Python
•	C#
•	Core Java
•	JSON
•	PHP
•	.NET Framework
•	Xamarin Framework
•	Web Development
•	Google Accessibility Packages
 
    - **Devesh Shyngle** **(** Programmer & Security Professional **)**
•	C#
•	C++
•	PHP
•	.NET Framework
•	Xamarin Framework
•	Google Play Services
•	Web Development
•	Security Optimizer
•	Bug Tester

    - **Shubham Dwivedi** **(** Assistant Programmer **)**
•	C#
•	C++
•	.NET Framework
•	Xamarin Framework
•	Google Play Services

## 9.	Team's Achievements and Experience
- ### Rajasthan Hackathon 2017 (Project Link)
    - Developed a Stand-Alone Windows application which incorporated 2 leading User Services, “Bhamashah” and “e-Mitra” to create a User Friendly and Efficient application which focuses on performing major tasks with little-or-no Extra Inputs from User ensuring minimal interaction from User side, yet performing crucial tasks.
    
    - Stood among TOP 50 participants in the Hackathon. 


- ### ICICI Hackathon 2017
    - Developed an Android application which introduces a new Method for Visually-Challenged People to interact and use Bank Services such as performing Fund Transfers, Bill Payment and checking Account Details.

## 10.	Team
### Team Name : Psychotic Elites
- #### Dhruv Kanojia ([Lead Developer](https://github.com/Xonshiz))
- #### Ankit Pass ([UI/UX Designer](https://github.com/ankitpassi141))
- #### Devesh Shyngle ([Programmer & Security Tester](https://github.com/deveshyngle))
- #### Shubham Dwivedi ([Assistant Programmer](https://github.com/shubham1706))
