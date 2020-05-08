#!groovy
import groovy.json.JsonOutput
import groovy.json.JsonSlurper
def label, tag, name, registry, project_name, ip, port, storage_size, storage_type, cpu_size, ram_size
// Please change these varialbes to reflect your project
label = 'builder'
tag = 'latest'
name = 'gear'
project_name = 'dotnet-dev-gear'
registry = "gear/gr"
ip = '192.168.1.45'
port = '8383'
storage_type = 'nfs'
storage_size = '5Gi'
cpu_size = "100m"
ram_size = "1024Mi"
//end variables

timestamps {

  podTemplate(label: label,
         containers: [
            containerTemplate(name: 'jnlp', image: 'jenkins/jnlp-slave:alpine'),
            containerTemplate(name: 'docker', image: 'docker', command: 'cat', ttyEnabled: true),
  ],
            volumes: [hostPathVolume(hostPath: '/var/run/docker.sock', mountPath: '/var/run/docker.sock')]     
    )

    {

    node(label) {
   
    try {
    def app
    
    stage('Clone repository') {
        
        checkout scm
        
    }

    stage('Build image') {
        
           container('docker') {
                echo "Building docker image..."                
                app = docker.build("${registry}", "--network=host .")
         }
 
    }

    stage('Test image with SonarQube') {
        
        def scannerHome = tool 'sonarScanner';
            withSonarQubeEnv('sonar') { 
            sh "${scannerHome}/bin/sonar-scanner -Dsonar.projectKey=${project_name}"
    }
       

    }

    stage('Push image to Docker Registry') {
        container('docker') {
        
        docker.withRegistry("https://gitlab.dev.indrivo.com:5005/${registry}", "docker_registry") {
                    app.push("${tag}") 
        }
            
        }
    }
    currentBuild.result = 'SUCCESS'
    } catch(e) {
    currentBuild.result = 'FAILURE'
    throw e
    }
    }

    node{

    stage('Clone repository') {
        
        checkout scm
       
    }

    stage('Deploy to K8s cluster') {
    withKubeConfig([credentialsId: 'kube2',serverUrl: 'https://192.168.1.40:6443',clusterName: 'kubernetes',
                    namespace: 'development']) {
                        
       
    sh "kubectl delete all,ing -l app=${project_name} -n development"
              
    
    script{
                   def image_id = "gitlab.dev.indrivo.com:5005/${registry}" + ":" + tag
                   sh "ansible-playbook  playbook.yml --extra-vars \"image_id=${image_id} name=${name} project_name=${project_name} ip=${ip} port=${port} size=${storage_size} cpu=${cpu_size} ram=${ram_size}\""
    }
        
    
        }
        
    }
    stage('Cleanup'){
     
        cleanWs deleteDirs: true, notFailBuild: true
    }

    stage('Report') {

    echo "${name} has been deloyed at the following address: http://${ip}:${port}/"
    echo "${name} sonar report can be found here: http://sonar.dev.indrivo.com/dashboard?id=${project_name}"
    }
    
    

}

    }
}