
module "kube_prometheus_stack" {
  source  = "terraform-helm-charts/kube-prometheus-stack/helm"
  version = "1.1.3"

  name       = "kps"
  namespace  = "monitoring"
  create_namespace = true
  values = [<<EOF
prometheus:
  prometheusSpec:
    serviceMonitorSelectorNilUsesHelmValues: false
grafana:
  adminPassword: "midazgrafana"
  service:
    type: LoadBalancer
EOF
  ]
}

module "tempo" {
  source  = "terraform-helm-charts/grafana-tempo/helm"
  version = "1.3.2"

  name      = "tempo"
  namespace = "monitoring"
  values = [<<EOF
tempo:
  metricsGenerator:
    enabled: true
EOF
  ]
}

module "loki" {
  source  = "terraform-helm-charts/loki-stack/helm"
  version = "2.9.10"

  name      = "loki"
  namespace = "monitoring"
  values = [<<EOF
loki:
  auth_enabled: false
grafana:
  enabled: false
prometheus:
  enabled: false
EOF
  ]
}

module "opencost" {
  source  = "terraform-helm-charts/opencost/helm"
  version = "1.0.4"

  name      = "opencost"
  namespace = "monitoring"
}
