
cmake_minimum_required( VERSION 3.8 )
project( "mgtest" CSharp )

include( CSharpUtilities )

file( GLOB SOURCE_FILES "*.cs" )

add_executable( ${PROJECT_NAME}
  ${SOURCE_FILES}
  Properties/AssemblyInfo.cs
  )

target_compile_options( ${PROJECT_NAME} PRIVATE "/langversion:6" )

csharp_set_designer_cs_properties(
  Properties/AssemblyInfo.cs
  )

set_property( TARGET ${PROJECT_NAME} PROPERTY VS_DOTNET_TARGET_FRAMEWORK_VERSION "v4.5")
set_property( TARGET ${PROJECT_NAME} PROPERTY WIN32_EXECUTABLE TRUE)

#set_property( TARGET ${PROJECT_NAME} PROPERTY VS_DOTNET_REFERENCES
#  "MonoGame.Framework"
#  )

find_program( NUGET nuget )
add_custom_target( nuget-restore
  COMMAND ${NUGET} restore ${CMAKE_CURRENT_BINARY_DIR}/${PROJECT_NAME}.sln
  )

configure_file( ${CMAKE_CURRENT_SOURCE_DIR}/packages.config
  ${CMAKE_CURRENT_BINARY_DIR}/packages.config COPYONLY
  )

set_target_properties( ${PROJECT_NAME} PROPERTIES
  VS_GLOBAL_ROOTNAMESPACE ${PROJECT_NAME}
  VS_GLOBAL_ProjectGuid "{f84e584d-a422-3021-a20c-cec777413cae}"
  VS_DOTNET_TARGET_FRAMEWORK_VERSION "v4.5"
  VS_DOTNET_REFERENCES
  "${CMAKE_CURRENT_BINARY_DIR}/packages/MonoGame.Framework.WindowsDX.3.6.0.1625/lib/net40/MonoGame.Framework.dll"
  )

add_dependencies( ${PROJECT_NAME} nuget-restore )

add_custom_target( ContentOutput
                   ALL
                   WORKING_DIRECTORY ../Content
                   COMMAND mgcb.exe /platform:Windows /@:Content.mgcb
                   DEPENDS ${PROJECT_NAME}
                   SOURCES
                   ../Content/Effects/Ambient.fx
                   ../Content/Effects/NightDay.fx
                   ../Content/Effects/SimpleEffect.fx
                   ../Content/Models/brushed_.jpg
                   ../Content/Models/Earthmodel.fbx
                   ../Content/Models/Helicopter.fbx
                   ../Content/Models/HelicopterHeightMap.tga
                   ../Content/Models/HelicopterNormalMap.tga
                   ../Content/Models/HelicopterTexture.png
                   ../Content/Textures/earth2k.jpg
                   ../Content/Textures/WorldDay.jpg
                   ../Content/Textures/WorldNight.jpg
                   )


add_custom_command( TARGET ${PROJECT_NAME} POST_BUILD
                    COMMAND ${CMAKE_COMMAND} -E copy_directory ${CMAKE_SOURCE_DIR}/Content/bin/Windows $<TARGET_FILE_DIR:${PROJECT_NAME}>/Content
                    )
